using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PhotoCameraController : MonoBehaviour
{
    [Header("Camera Interaction")]
    private bool m_cameraActivated = false;
    public InputActionReference m_zoomInput = null;

    [Header("zoom")]
    [SerializeField] private float m_zoomFactor = 2.0f;
    [SerializeField] private float m_zoomLerpSpeed = 1.0f;
    [Tooltip("Smaller number means zoom in")]
    [SerializeField] private Vector2 m_zoomLimit = new Vector2(10, 80);
    [SerializeField] private XRBaseController leftController;
    [SerializeField] private float vibrationInterval = 0.2f;
    [SerializeField] private float vibrationAmplitude = 0.2f;
    [SerializeField] private float vibrationDuration = 0.01f;
    private float timeSinceLastVibration;

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
        timeSinceLastVibration += Time.deltaTime;

        float zoomInOut = -m_zoomInput.action.ReadValue<Vector2>().y;

        // if the zoom level is changing
        if (timeSinceLastVibration >= vibrationInterval)
        {
            if (zoomInOut != 0)
            {
                timeSinceLastVibration = 0.0f;
                leftController.SendHapticImpulse(vibrationAmplitude, vibrationDuration);
            }
        }

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
