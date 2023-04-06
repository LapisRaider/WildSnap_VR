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
    public GameObject m_photoScreen;
    public float m_maxAnimalDistance;
    public int m_raysShotPerAnimal;
    public float m_rayThreshold;
    public float m_imageSizeThreshold;
    public LineRenderer m_outlineIndicator;

    private WaitForEndOfFrame m_enumeratorEndOfFrame = new WaitForEndOfFrame();
    private Rect m_regionToRead;
    private int m_currPhotoCount = 0;
    private bool m_isTakingPhoto = false;
    private float m_photoScreenWidth;
    private float m_photoScreenHeight;
    private int m_raysShotPerAnimalPerAxis;

    [Header("On Take Photo")]
    public ParticleSystem m_flashParticles; 
    public Animator m_flashAnim;
    public AudioSource m_flashAudioSource;

    // Start is called before the first frame update
    void Awake()
    {
        m_regionToRead = new Rect(0, 0, m_photoTargetTexture.width, m_photoTargetTexture.height);
        Vector3 photoScreenSize = m_photoScreen.GetComponent<Renderer>().localBounds.size;

        m_photoScreenWidth = photoScreenSize.x * m_photoScreen.transform.localScale.x;
        m_photoScreenHeight = photoScreenSize.y * m_photoScreen.transform.localScale.y;
        m_outlineIndicator.SetWidth(0.001f, 0.001f);

        // round down to the nearest cube number to do uniform sampling (so the score doesn't flicker)
        m_raysShotPerAnimalPerAxis = (int)Mathf.Pow(m_raysShotPerAnimal, 1f / 3f);
        m_raysShotPerAnimal = m_raysShotPerAnimalPerAxis * m_raysShotPerAnimalPerAxis * m_raysShotPerAnimalPerAxis;
    }

    // uncomment if too laggy
    // void Start()
    // {
    //     InvokeRepeating("DrawIndicatorOnCamera", 0.2f, 0.2f);
    // }

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
        // comment out if too laggy, use InvokeRepeating instead of calling every frame
        DrawIndicatorOnCamera();
        if (m_takePhoto.action.triggered) OnTakePhoto();
    }

    private void DrawIndicatorOnCamera()
    {
        Animal_Behaviour animal;
        // should all be from -0.5 to 0.5
        float minX;
        float maxX;
        float minY;
        float maxY;
        float photoScore = GetPhotoScoreAndStats(out animal, out minX, out maxX, out minY, out maxY);

        if (animal == null)
        {
            m_outlineIndicator.enabled = false;
        } else 
        {
            Vector3[] linePositions = {
                new Vector3(maxX * m_photoScreenWidth, maxY * m_photoScreenHeight, 0),
                new Vector3(minX * m_photoScreenWidth, maxY * m_photoScreenHeight, 0),
                new Vector3(minX * m_photoScreenWidth, minY * m_photoScreenHeight, 0),
                new Vector3(maxX * m_photoScreenWidth, minY * m_photoScreenHeight, 0),
            };
            m_outlineIndicator.SetPositions(linePositions);

            if (photoScore > 1000)
            {
                m_outlineIndicator.startColor = Color.green;
                m_outlineIndicator.endColor = Color.green;
            } else if (photoScore > 500)
            {
                m_outlineIndicator.startColor = Color.yellow;
                m_outlineIndicator.endColor = Color.yellow;
            } else
            {
                m_outlineIndicator.startColor = Color.red;
                m_outlineIndicator.endColor = Color.red;
            }
            
            m_outlineIndicator.enabled = true;
        }
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, List<Camera> arg2)
    {
        if (!m_isTakingPhoto)
            return;

        m_isTakingPhoto = false;

        Animal_Behaviour animal;
        float minX;
        float maxX;
        float minY;
        float maxY;
        float photoScore = GetPhotoScoreAndStats(out animal, out minX, out maxX, out minY, out maxY);

        if (photoScore == 0.0f) return;
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

    public void OnTakePhoto()
    {
        m_isTakingPhoto = true;

        if (m_flashParticles != null) m_flashParticles.Play();
    
        if (m_flashAnim != null) m_flashAnim.SetTrigger("Flash");

        if (m_flashAudioSource != null) m_flashAudioSource.Play();
    }

    public float CalculatePhotoScore(
        AnimalType type, AnimalState animalState, float rayHitProportion, float distance, 
        float imageSize, float distFromCenter, float facingCamera, int animalsInFrame)
    {
        float score = 100.0f;

        float stateScoreMultiplier = 1.0f;
        AnimalDexEntry dexEntry = AnimalDex.Instance.GetAnimalDexEntry(type);
        if (dexEntry != null && dexEntry.m_photoStateScoreMap.ContainsKey(animalState))
        {
            stateScoreMultiplier = dexEntry.m_photoStateScoreMap[animalState];
        } else 
        {
            Debug.LogError("Cannot find multiplier for animal state");
        }
        score *= stateScoreMultiplier;

        // scale 0 - 0.3 to 1 - 3, and 0.3 - 1 to 3
        // bigger image should give more score, up to taking up 0.3 of the screen
        float imageSizeMultiplier = Mathf.Max(1.0f, Math.Min(imageSize * 10, 3.0f));
        score *= imageSizeMultiplier;

        // scale 0 - MAX_DISTANCE to 1 - 2, closer image should give more score
        float distanceMultiplier = Mathf.Max(1.0f, -2 * distance / m_maxAnimalDistance + 2);
        score *= distanceMultiplier;

        // scale 0 - 1 to 1 - 2, more ray hits should give more score
        float rayHitMultiplier = Mathf.Max(1.0f, rayHitProportion * 2);
        score *= rayHitMultiplier;

        // scale 0 - 1 to 1 - 3, facing you should give more score
        float facingCameraMultiplier = Mathf.Max(1.0f, facingCamera * 3);
        score *= facingCameraMultiplier;

        // scale 0 - maxDist to 1 - 3, closer should give more score
        float maxDistFromCenter = Mathf.Sqrt(
            (m_photoScreenWidth / 2.0f) * (m_photoScreenWidth / 2.0f) +
            (m_photoScreenHeight / 2.0f) * (m_photoScreenHeight / 2.0f)
        );
        float distFromCenterMultiplier = Mathf.Max(1.0f, -3 * distFromCenter / maxDistFromCenter + 3);
        score *= distFromCenterMultiplier;

        // +10% score per other animal
        float animalsInFrameMultiplier = 1.0f + 0.1f * (animalsInFrame - 1);
        score *= animalsInFrameMultiplier;

        return score;
    }

    public void DetectFocusAnimal(
        out GameObject animal, out int raysHit, out float distance, 
        out float minX, out float maxX, out float minY,
        out float maxY, out float facingCamera, out int animalsInFrame)
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
        float foundMinX = 0;
        float foundMaxX = 0;
        float foundMinY = 0;
        float foundMaxY = 0;

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

            float screenMinX = 1.0f;
            float screenMaxX = 0.0f;
            float screenMinY = 1.0f;
            float screenMaxY = 0.0f;

            foreach (Vector3 corner in boundsCorners)
            {
                Vector3 viewportCoords = m_photoTakingCamera.WorldToViewportPoint(corner);
                screenMinX = Mathf.Min(screenMinX, viewportCoords.x);
                screenMaxX = Mathf.Max(screenMaxX, viewportCoords.x);
                screenMinY = Mathf.Min(screenMinY, viewportCoords.y);
                screenMaxY = Mathf.Max(screenMaxY, viewportCoords.y);
            }
            
            screenMinX = Mathf.Max(screenMinX, 0.0f);
            screenMaxX = Mathf.Min(screenMaxX, 1.0f);
            screenMinY = Mathf.Max(screenMinY, 0.0f);
            screenMaxY = Mathf.Min(screenMaxY, 1.0f);

            float imageSize = (screenMaxX - screenMinX) * (screenMaxY - screenMinY);
            // if animal takes up too little space in the screen, reject
            if (imageSize < m_imageSizeThreshold) continue;

            // counts number of animals that are "large enough"
            // TODO: this does not shoot rays to the "other" animals, so it counts those occluded too
            animalsInFrame++;

            // only need to return the first animal that was found
            if (animalFound) continue;

            Vector3 rayStart = boundMin + collider.bounds.extents * 0.05f;
            Vector3 boundLength = collider.bounds.extents * 0.9f;
            Vector3 gridLength = boundLength / (float)m_raysShotPerAnimalPerAxis;
            rayStart += 0.5f * gridLength;

            int hitCount = 0;
            // don't worry about the triple loop it's not spaghetti
            for (int rayX = 0; rayX < m_raysShotPerAnimalPerAxis; rayX++)
            {
                for (int rayY = 0; rayY < m_raysShotPerAnimalPerAxis; rayY++)
                {
                    for (int rayZ = 0; rayZ < m_raysShotPerAnimalPerAxis; rayZ++)
                    {
                        Vector3 shootAt = rayStart + new Vector3(
                            rayX * gridLength.x, 
                            rayY * gridLength.y, 
                            rayZ * gridLength.z);

                        // if random point out of frustum. reject
                        Vector3 viewportCoords = m_photoTakingCamera.WorldToViewportPoint(shootAt);
                        if (viewportCoords.x < 0 || viewportCoords.x > 1 || 
                            viewportCoords.y < 0 || viewportCoords.y > 1 || viewportCoords.z < 0) 
                        {
                            continue;
                        }

                        RaycastHit hit;
                        bool hasHit = Physics.Raycast(
                            m_photoTakingCamera.transform.position,
                            shootAt - m_photoTakingCamera.transform.position,
                            out hit
                        );

                        if (hasHit && collider.bounds.Contains(hit.point))
                        {
                            hitCount++;
                        }
                    }
                }
            }

            if (hitCount > m_rayThreshold * m_raysShotPerAnimal)
            {
                // the first one that reaches this is the most centered animal that is highly visible
                animalFound = true;
                // scale to -0.5 to 0.5
                foundMinX = screenMinX - 0.5f;
                foundMaxX = screenMaxX - 0.5f;
                foundMinY = screenMinY - 0.5f;
                foundMaxY = screenMaxY - 0.5f;
                foundRaysHit = hitCount;
                foundAnimal = collider.gameObject;
                Vector3 closestPoint = collider.ClosestPoint(m_photoTakingCamera.transform.position);
                foundDistance = (closestPoint - m_photoTakingCamera.transform.position).magnitude;
                
                Vector3 animalForward = foundAnimal.transform.forward;
                foundFacingCamera = Vector3.Dot(-animalForward, cameraFrontVector);
                foundFacingCamera = Math.Max(0.0f, foundFacingCamera);
            }
        }

        animal = foundAnimal;
        raysHit = foundRaysHit;
        distance = foundDistance;
        minX = foundMinX;
        maxX = foundMaxX;
        minY = foundMinY;
        maxY = foundMaxY;
        facingCamera = foundFacingCamera;
        return;
    }

    public float GetPhotoScoreAndStats(
        out Animal_Behaviour animal, out float minX, out float maxX, 
        out float minY, out float maxY)
    {
        animal = null;
        GameObject focusAnimal;
        int raysHit;
        float distance;
        float facingCamera;
        int animalsInFrame;

        DetectFocusAnimal(
            out focusAnimal, out raysHit, out distance, out minX, out maxX, 
            out minY, out maxY, out facingCamera, out animalsInFrame
        );

        float imageSize = (maxX - minX) * (maxY - minY);

        float centerX = ((maxX - minX) / 2.0f) * m_photoScreenWidth;
        float centerY = ((maxY - minY) / 2.0f) * m_photoScreenHeight;
        float distFromCenter = Mathf.Sqrt(centerX * centerX + centerY * centerY);

        float rayHitProportion = (float)raysHit / m_raysShotPerAnimal;

        if (focusAnimal == null)
            return 0.0f;

        animal = focusAnimal.GetComponent<Animal_Behaviour>();
        if (animal == null)
        {
            Debug.LogError("This animal does not have the Animal_Behaviour component: " + focusAnimal.name);
            return 0.0f;
        }

        return CalculatePhotoScore(
            animal.m_animalType, animal.GetAnimalState(), rayHitProportion, 
            distance, imageSize, distFromCenter, facingCamera, animalsInFrame
        );
    }
}
