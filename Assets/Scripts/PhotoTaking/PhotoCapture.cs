using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class PhotoCapture : MonoBehaviour
{
    public RenderTexture m_photoTargetTexture;
    public InputActionReference m_takePhoto = null;

    private WaitForEndOfFrame m_enumeratorEndOfFrame = new WaitForEndOfFrame();
    public Camera m_photoTakingCamera;
    public float m_maxAnimalDistance;
    public int m_raysShotPerAnimal;
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
        float stateScore = AnimalDex.Instance.GetAnimalDexEntry(type).m_photoStateScoreMap[animalState];

        // scale this based on animal properties i guess?
        // ideas: rarity, how much of the frame contains the animal, etc.
        return rayHitProportion * distance * imageSize * baseScore * stateScore;
    }

    public void DetectFocusAnimal(out GameObject animal, out int raysHit, out float distance, out float imageSize)
    {
        Vector3 cameraFrontVector = m_photoTakingCamera.transform.forward;
        Collider[] collidersInRadius = Physics.OverlapSphere(m_photoTakingCamera.transform.position, m_maxAnimalDistance);
        collidersInRadius = collidersInRadius.Where(collider => collider.tag == "Animal").ToArray();
        
        float[] colliderDots = collidersInRadius.Select(
            collider => Vector3.Dot(cameraFrontVector, (collider.transform.position - m_photoTakingCamera.transform.position).normalized)
        ).ToArray();
        // sort the colliders based on the dot values
        Array.Sort(colliderDots, collidersInRadius);

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(m_photoTakingCamera);

        for (int i = collidersInRadius.Length - 1; i >= 0; i--)
        {
            Collider collider = collidersInRadius[i];
            float dot = colliderDots[i];
            if (dot < 0) continue;
            // more accurate check that the animal is in the frustum
            if (!GeometryUtility.TestPlanesAABB(planes, collider.bounds)) continue;

            int hitCount = 0;
            // randomly sample the aabb and see if how many hit the animal
            for (int j = 0; j < m_raysShotPerAnimal; j++)
            {
                Vector3 randomPointInBounds = new Vector3(
                    UnityEngine.Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                    UnityEngine.Random.Range(collider.bounds.min.y, collider.bounds.max.y),
                    UnityEngine.Random.Range(collider.bounds.min.z, collider.bounds.max.z)
                );
                RaycastHit hit;
                Physics.Raycast(
                    m_photoTakingCamera.transform.position,
                    randomPointInBounds - m_photoTakingCamera.transform.position,
                    out hit
                );
                if (collider.bounds.Contains(hit.point))
                {
                    hitCount++;
                }
            }
            
            if (hitCount > 0.5f * m_raysShotPerAnimal)
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
                Matrix4x4 viewMat = m_photoTakingCamera.worldToCameraMatrix;
                Matrix4x4 projMat = m_photoTakingCamera.projectionMatrix;

                float screenMinX = Mathf.Infinity;
                float screenMaxX = -Mathf.Infinity;
                float screenMinY = Mathf.Infinity;
                float screenMaxY = -Mathf.Infinity;

                foreach (Vector3 corner in boundsCorners)
                {
                    Vector4 homo = new Vector4(corner.x, corner.y, corner.z, 1);
                    Vector4 screenSpace = projMat * viewMat * homo;
                    screenMinX = Mathf.Min(screenMinX, screenSpace.x);
                    screenMaxX = Mathf.Max(screenMaxX, screenSpace.x);
                    screenMinY = Mathf.Min(screenMinY, screenSpace.y);
                    screenMaxY = Mathf.Max(screenMaxY, screenSpace.y);
                }

                imageSize = (screenMaxX - screenMinX) * (screenMaxY - screenMinY);
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
