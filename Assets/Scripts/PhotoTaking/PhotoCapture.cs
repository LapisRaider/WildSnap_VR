using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    public RenderTexture m_photoTargetTexture;

    private WaitForEndOfFrame m_enumeratorEndOfFrame = new WaitForEndOfFrame();
    public Camera m_photoTakingCamera;
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

        RawImage img = imageObj.GetComponent<RawImage>();
        img.texture = newImage;
    }

    public void TakePhoto()
    {
        m_isTakingPhoto = true;

        if (m_testCaptureParticle != null)
            m_testCaptureParticle.Play();
    }
}
