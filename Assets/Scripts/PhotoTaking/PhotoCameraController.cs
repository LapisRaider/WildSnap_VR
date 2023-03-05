using UnityEngine;
using UnityEngine.InputSystem;

public class PhotoCameraController : MonoBehaviour
{
    [Header("Camera Interaction")]
    private bool m_cameraActivated = false;
    public InputActionReference m_zoomInInput = null;
    public InputActionReference m_zoomOutInput = null;

    [Header("zoom")]
    [SerializeField] private float m_zoomFactor = 2.0f;
    [SerializeField] private float m_zoomLerpSpeed = 1.0f;
    [Tooltip("Smaller number means zoom in")]
    [SerializeField] private Vector2 m_zoomLimit = new Vector2(10, 80);

    private float m_targetZoomAmt = 0.0f;
    private Camera m_photoTakingCamera;

    // Start is called before the first frame update
    void Awake()
    {
        m_photoTakingCamera = GetComponent<Camera>();
        m_targetZoomAmt = m_photoTakingCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_cameraActivated)
            return;

        int zoomInOut = m_zoomInInput.action.ReadValue<float>() > 0.1f ? -1 : m_zoomOutInput.action.ReadValue<float>() > 0.1f ? 1 : 0;

        //smaller FOV is zoom in
        m_targetZoomAmt += zoomInOut * m_zoomFactor;
        m_targetZoomAmt = Mathf.Clamp(m_targetZoomAmt, m_zoomLimit.x, m_zoomLimit.y);

        m_photoTakingCamera.fieldOfView = Mathf.Lerp(m_photoTakingCamera.fieldOfView, m_targetZoomAmt, Time.deltaTime * m_zoomLerpSpeed);
    }

    public void SetActivation(bool isActivate)
    {
        m_cameraActivated = isActivate;
    }
}
