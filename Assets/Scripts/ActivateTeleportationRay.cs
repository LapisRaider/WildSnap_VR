using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateTeleportationRay : MonoBehaviour
{
    public GameObject m_teleportationRay;

    public InputActionProperty m_teleportActivation;
    public float m_teleportTriggerThreshold = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_teleportationRay.SetActive(m_teleportActivation.action.ReadValue<float>() > m_teleportTriggerThreshold);
    }
}
