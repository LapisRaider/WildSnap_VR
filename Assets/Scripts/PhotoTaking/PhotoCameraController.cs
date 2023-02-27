using UnityEngine;

public class PhotoCameraController : MonoBehaviour
{
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
        //TODO: change input to VR controller joystick
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        //smaller FOV is zoom in
        m_targetZoomAmt -= scrollData * m_zoomFactor;
        m_targetZoomAmt = Mathf.Clamp(m_targetZoomAmt, m_zoomLimit.x, m_zoomLimit.y);

        m_photoTakingCamera.fieldOfView = Mathf.Lerp(m_photoTakingCamera.fieldOfView, m_targetZoomAmt, Time.deltaTime * m_zoomLerpSpeed);
    }
}
