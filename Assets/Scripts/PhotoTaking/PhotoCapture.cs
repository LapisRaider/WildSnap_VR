using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class ReverseSortFloats : IComparer<float>
{
    int IComparer<float>.Compare(float x, float y)
    {
        return x > y ? -1 : x < y ? 1 : 0;
    }
}

public class PhotoCapture : MonoBehaviour
{
    public RenderTexture m_photoTargetTexture;
    public InputActionReference m_takePhoto = null;
    public Camera m_photoTakingCamera;
    public float m_maxAnimalDistance;
    public int m_raysShotPerAnimal;
    public float m_rayThreshold;
    public float m_imageSizeThreshold;

    private WaitForEndOfFrame m_enumeratorEndOfFrame = new WaitForEndOfFrame();
    private Rect m_regionToRead;
    private int m_currPhotoCount = 0;
    private bool m_isTakingPhoto = false;

    //TODO: THIS IS FOR TESTING, REMOVE LATER
    public ParticleSystem m_testCaptureParticle; 

    // Start is called before the first frame update
    void Awake()
    {
        m_regionToRead = new Rect(0, 0, m_photoTargetTexture.width, m_photoTargetTexture.height);
    }

    private void OnEnable()
    {
        RenderPipelineManager.endContextRendering += RenderPipelineManager_endCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endContextRendering -= RenderPipelineManager_endCameraRendering;
    }

    private void Update()
    {
        if (m_takePhoto.action.triggered)
            TakePhoto();
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, List<Camera> arg2)
    {
        if (!m_isTakingPhoto)
            return;

        m_isTakingPhoto = false;

        //check whether an animal is in the photo
        GameObject focusAnimal;
        int raysHit;
        float distance;
        float imageSize;

        DetectFocusAnimal(out focusAnimal, out raysHit, out distance, out imageSize);

        if (focusAnimal == null)
            return;

        Animal_Behaviour animal = focusAnimal.GetComponent<Animal_Behaviour>();
        if (animal == null)
        {
            Debug.LogError("This animal does not have the Animal_Behaviour component: " + focusAnimal.name);
            return;
        }

        float photoScore = CalculatePhotoScore(animal.m_animalType, animal.GetAnimalState(), (float)raysHit / m_raysShotPerAnimal, distance, imageSize);
        AddPhotoToUiAlbum(animal, photoScore);
    }

    private void AddPhotoToUiAlbum(Animal_Behaviour animal, float photoScore)
    {
        //read and save photo
        Texture2D photoTaken = new Texture2D(m_photoTargetTexture.width, m_photoTargetTexture.height, TextureFormat.ARGB32, false);
        m_regionToRead = new Rect(0, 0, m_photoTargetTexture.width, m_photoTargetTexture.height);
        photoTaken.ReadPixels(m_regionToRead, 0, 0);
        photoTaken.Apply();

        AnimalDex.Instance.AddPhotoToDexEntry(animal.m_animalType, animal.GetAnimalState(), (int)photoScore, photoTaken);

        //SavePhotosToFolder(photoTaken);
    }

    private void SavePhotosToFolder(Texture2D photoTaken)
    {
        byte[] byteArray = photoTaken.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Photos/photo" + m_currPhotoCount + ".png", byteArray);

        ++m_currPhotoCount;
    }

    public void TakePhoto()
    {
        m_isTakingPhoto = true;

        if (m_testCaptureParticle != null)
            m_testCaptureParticle.Play();
    }

    public float CalculatePhotoScore(AnimalType type, AnimalState animalState, float rayHitProportion, float distance, float imageSize)
    {
        float baseScore = 10.0f;
        float stateScoreMultiplier = AnimalDex.Instance.GetAnimalDexEntry(type).m_photoStateScoreMap[animalState];

        // scale 0 - 1 to 1 - 30, bigger image should give more score
        float imageSizeMultiplier = Mathf.Max(1.0f, imageSize * 30);

        // scale 0 - MAX_DISTANCE to 1 - 10, closer image should give more score
        float distanceMultiplier = Mathf.Max(1.0f, -10 * distance / m_maxAnimalDistance + 10);

        // scale 0 - 1 to 1 - 30, more ray hits should give more score
        float rayHitMultiplier = Mathf.Max(1.0f, rayHitProportion * 30);

        return rayHitProportion * imageSizeMultiplier * distanceMultiplier * rayHitMultiplier * stateScoreMultiplier;
    }

    public void DetectFocusAnimal(out GameObject animal, out int raysHit, out float distance, out float imageSize)
    {
        Vector3 cameraFrontVector = m_photoTakingCamera.transform.forward;
        Collider[] collidersInRadius = Physics.OverlapSphere(m_photoTakingCamera.transform.position, m_maxAnimalDistance);
        collidersInRadius = collidersInRadius.Where(collider => collider.tag == "Animal").ToArray();
        
        float[] colliderDots = collidersInRadius.Select(
            collider => Vector3.Dot(cameraFrontVector, (collider.transform.position - m_photoTakingCamera.transform.position).normalized)
        ).ToArray();
        // sort the colliders based on descending dot values
        Array.Sort(colliderDots, collidersInRadius, new ReverseSortFloats());

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(m_photoTakingCamera);

        for (int i = 0; i < collidersInRadius.Length; i++)
        {
            Collider collider = collidersInRadius[i];
            float dot = colliderDots[i];
            if (dot < 0) continue;
            // more accurate check that the animal is in the frustum
            if (!GeometryUtility.TestPlanesAABB(planes, collider.bounds)) continue;

            int hitCount = 0;

            // TODO: this ray thing is the most unstable thing ever, find a way to get a "random" part on animal's mesh
            for (int j = 0; j < m_raysShotPerAnimal; j++)
            {
                Vector3 randomPointInBounds = new Vector3(
                    UnityEngine.Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                    UnityEngine.Random.Range(collider.bounds.min.y, collider.bounds.max.y),
                    UnityEngine.Random.Range(collider.bounds.min.z, collider.bounds.max.z)
                );

                // if random point out of frustum. reject
                Vector3 viewportCoords = m_photoTakingCamera.WorldToViewportPoint(randomPointInBounds);
                if (viewportCoords.x < 0 || viewportCoords.x > 1 || 
                    viewportCoords.y < 0 || viewportCoords.y > 1 || viewportCoords.z < 0) 
                {
                    continue;
                }

                RaycastHit hit;
                bool hasHit = Physics.Raycast(
                    m_photoTakingCamera.transform.position,
                    randomPointInBounds - m_photoTakingCamera.transform.position,
                    out hit
                );

                if (hasHit && collider.bounds.Contains(hit.point))
                {
                    hitCount++;
                }
            }
            
            if (hitCount > m_rayThreshold * m_raysShotPerAnimal)
            {
                // the first one that reaches this is the most centered animal that is highly visible
                raysHit = hitCount;
                animal = collider.gameObject;
                Vector3 closestPoint = collider.ClosestPoint(m_photoTakingCamera.transform.position);
                distance = (closestPoint - m_photoTakingCamera.transform.position).sqrMagnitude;

                // use the bounds to get an estimate of how much space the image takes
                Vector3[] boundsCorners = {
                    new Vector3(collider.bounds.min.x, collider.bounds.min.y, collider.bounds.min.z),
                    new Vector3(collider.bounds.min.x, collider.bounds.min.y, collider.bounds.max.z),
                    new Vector3(collider.bounds.min.x, collider.bounds.max.y, collider.bounds.min.z),
                    new Vector3(collider.bounds.max.x, collider.bounds.min.y, collider.bounds.min.z),
                    new Vector3(collider.bounds.min.x, collider.bounds.max.y, collider.bounds.max.z),
                    new Vector3(collider.bounds.max.x, collider.bounds.max.y, collider.bounds.min.z),
                    new Vector3(collider.bounds.max.x, collider.bounds.min.y, collider.bounds.max.z),
                    new Vector3(collider.bounds.max.x, collider.bounds.max.y, collider.bounds.max.z)
                };

                float screenMinX = 1;
                float screenMaxX = 0;
                float screenMinY = 1;
                float screenMaxY = 0;

                foreach (Vector3 corner in boundsCorners)
                {
                    Vector3 viewportCoords = m_photoTakingCamera.WorldToViewportPoint(corner);
                    screenMinX = Mathf.Min(screenMinX, viewportCoords.x);
                    screenMaxX = Mathf.Max(screenMaxX, viewportCoords.x);
                    screenMinY = Mathf.Min(screenMinY, viewportCoords.y);
                    screenMaxY = Mathf.Max(screenMaxY, viewportCoords.y);
                }

                imageSize = (screenMaxX - screenMinX) * (screenMaxY - screenMinY);
                // if animal takes up too little space in the screen, reject
                if (imageSize < m_imageSizeThreshold) {
                    continue;
                }
                return;
            }
        }
        animal = null;
        raysHit = 0;
        distance = 0;
        imageSize = 0;
        return;
    }
}
