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
        float facingCamera;
        int animalsInFrame;

        DetectFocusAnimal(out focusAnimal, out raysHit, out distance, out imageSize, out facingCamera, out animalsInFrame);

        if (focusAnimal == null)
            return;

        Animal_Behaviour animal = focusAnimal.GetComponent<Animal_Behaviour>();
        if (animal == null)
        {
            Debug.LogError("This animal does not have the Animal_Behaviour component: " + focusAnimal.name);
            return;
        }

        float photoScore = CalculatePhotoScore(animal.m_animalType, animal.GetAnimalState(), (float)raysHit / m_raysShotPerAnimal, distance, imageSize, facingCamera, animalsInFrame);
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

    public float CalculatePhotoScore(AnimalType type, AnimalState animalState, float rayHitProportion, float distance, float imageSize, float facingCamera, int animalsInFrame)
    {
        float baseScore = 10.0f;

        float stateScoreMultiplier = 1.0f;
        AnimalDexEntry dexEntry = AnimalDex.Instance.GetAnimalDexEntry(type);
        if (dexEntry != null && dexEntry.m_photoStateScoreMap.ContainsKey(animalState))
        {
            stateScoreMultiplier = dexEntry.m_photoStateScoreMap[animalState];
        } else 
        {
            Debug.LogError("Cannot find multiplier for animal state");
        }

        // scale 0 - 0.5 to 1 - 5, and 0.5 - 1 to 5
        // bigger image should give more score, up to taking up 50% of the screen
        float imageSizeMultiplier = Mathf.Max(1.0f, Math.min(imageSize * 10, 5.0f));

        // scale 0 - MAX_DISTANCE to 1 - 5, closer image should give more score
        float distanceMultiplier = Mathf.Max(1.0f, -5 * distance / m_maxAnimalDistance + 5);

        // scale 0 - 1 to 1 - 5, more ray hits should give more score
        float rayHitMultiplier = Mathf.Max(1.0f, rayHitProportion * 10);

        // scale 0 - 1 to 1 - 5, facing you should give more score
        float facingCameraMultiplier = Mathf.Max(1.0f, facingCamera * 10);

        // +5% score per other animal
        float animalsInFrameMultiplier = 1.0f + 0.05f * animalsInFrame;

        return baseScore * rayHitProportion * imageSizeMultiplier * distanceMultiplier * rayHitMultiplier * stateScoreMultiplier * facingCameraMultiplier * animalsInFrameMultiplier;
    }

    public void DetectFocusAnimal(out GameObject animal, out int raysHit, out float distance, out float imageSize, out float facingCamera, out int animalsInFrame)
    {
        Vector3 cameraFrontVector = m_photoTakingCamera.transform.forward;
        Collider[] collidersInRadius = Physics.OverlapSphere(m_photoTakingCamera.transform.position, m_maxAnimalDistance);
        collidersInRadius = collidersInRadius.Where(collider => collider.tag == "Animal").ToArray();
        
        float[] colliderDots = collidersInRadius.Select(
            collider => Vector3.Dot(cameraFrontVector, (collider.transform.position - m_photoTakingCamera.transform.position).normalized)
        ).ToArray();
        // sort the colliders based on descending dot values
        Array.Sort(colliderDots, collidersInRadius, new ReverseSortFloats());

        animalsInFrame = 0;
        bool animalFound = false;
        GameObject foundAnimal = null;
        int foundRaysHit = 0;
        float foundDistance = 0;
        float foundImageSize = 0;
        float foundFacingCamera = 0;

        for (int i = 0; i < collidersInRadius.Length; i++)
        {
            Collider collider = collidersInRadius[i];
            float dot = colliderDots[i];
            // if behind camera, skip collider
            if (dot < 0) continue;

            Vector3 boundMin = collider.bounds.min;
            Vector3 boundMax = collider.bounds.max;
            // use the corners of the bounds to get an estimate of how much space the image takes in the viewport
            Vector3[] boundsCorners = {
                new Vector3(boundMin.x, boundMin.y, boundMin.z),
                new Vector3(boundMin.x, boundMin.y, boundMax.z),
                new Vector3(boundMin.x, boundMax.y, boundMin.z),
                new Vector3(boundMax.x, boundMin.y, boundMin.z),
                new Vector3(boundMin.x, boundMax.y, boundMax.z),
                new Vector3(boundMax.x, boundMax.y, boundMin.z),
                new Vector3(boundMax.x, boundMin.y, boundMax.z),
                new Vector3(boundMax.x, boundMax.y, boundMax.z)
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

            float thisImageSize = (screenMaxX - screenMinX) * (screenMaxY - screenMinY);
            // if animal takes up too little space in the screen, reject
            if (thisImageSize < m_imageSizeThreshold) continue;

            // counts number of animals that are "large enough"
            // TODO: this does not shoot rays to the "other" animals, so it counts those occluded too
            animalsInFrame++;

            // only need to return the first animal that was found
            if (animalFound) continue;

            Vector3 boundShrinkAmount = collider.bounds.extents * 0.05f;
            boundMin += boundShrinkAmount;
            boundMax -= boundShrinkAmount;

            int hitCount = 0;
            // shoot rays at the collider to determine if it is visible
            // TODO: shoot rays at hardcoded parts of the animal?
            for (int j = 0; j < m_raysShotPerAnimal; j++)
            {
                Vector3 randomPointInBounds = new Vector3(
                    UnityEngine.Random.Range(boundMin.x, boundMax.x),
                    UnityEngine.Random.Range(boundMin.y, boundMax.y),
                    UnityEngine.Random.Range(boundMin.z, boundMax.z)
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
                animalFound = true;
                foundImageSize = thisImageSize;
                foundRaysHit = hitCount;
                foundAnimal = collider.gameObject;
                Vector3 closestPoint = collider.ClosestPoint(m_photoTakingCamera.transform.position);
                foundDistance = (closestPoint - m_photoTakingCamera.transform.position).sqrMagnitude;
                
                Vector3 animalForward = foundAnimal.transform.forward;
                foundFacingCamera = Vector3.Dot(-animalForward, cameraFrontVector);
                foundFacingCamera = Math.Max(0.0f, foundFacingCamera);
            }
        }

        animal = foundAnimal;
        raysHit = foundRaysHit;
        distance = foundDistance;
        imageSize = foundImageSize;
        facingCamera = foundFacingCamera;
        return;
    }
}
