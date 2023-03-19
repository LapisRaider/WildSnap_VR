using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System;
using System.Linq;

public class PhotoCapture : MonoBehaviour
{
    public RenderTexture m_photoTargetTexture;

    private WaitForEndOfFrame m_enumeratorEndOfFrame = new WaitForEndOfFrame();
    public Camera m_photoTakingCamera;
    public float m_maxAnimalDistance;
    public int m_raysShotPerAnimal;
    private Rect m_regionToRead;
    private int m_currPhotoCount = 0;
    private bool m_isTakingPhoto = false;

    [Header("Phototaking Album")]
    public GameObject m_albumPanel;
    public GameObject m_photoPrefab;

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

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, List<Camera> arg2)
    {
        if (!m_isTakingPhoto)
            return;

        m_isTakingPhoto = false;

        Texture2D photoTaken = new Texture2D(m_photoTargetTexture.width, m_photoTargetTexture.height, TextureFormat.ARGB32, false);
        AddPhotoToUiAlbum(photoTaken);

        m_regionToRead = new Rect(0, 0, m_photoTargetTexture.width, m_photoTargetTexture.height);
        photoTaken.ReadPixels(m_regionToRead, 0, 0);
        photoTaken.Apply();

        byte[] byteArray = photoTaken.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Photos/photo" + m_currPhotoCount + ".png", byteArray);

        ++m_currPhotoCount;
    }

    private void AddPhotoToUiAlbum(Texture2D newImage)
    {
        //no storing photos
        if (m_albumPanel == null)
            return;

        GameObject imageObj = Instantiate(m_photoPrefab, m_albumPanel.transform);
        GameObject focusAnimal;
        int raysHit;
        float distance;
        float imageSize;
        DetectFocusAnimal(out focusAnimal, out raysHit, out distance, out imageSize);

        PhotoProperties photoProps = imageObj.GetComponent<PhotoProperties>();
        photoProps.SetImage(newImage);
        photoProps.SetAnimal(focusAnimal, (float)raysHit / m_raysShotPerAnimal, distance, imageSize);
    }

    public void TakePhoto()
    {
        m_isTakingPhoto = true;

        if (m_testCaptureParticle != null)
            m_testCaptureParticle.Play();
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
