using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateTeleportationRay : MonoBehaviour
{
    public GameObject m_teleportationRay;

    public InputActionProperty m_teleportActivation;
    public float m_teleportTriggerThreshold = 0.1f;

    [Tooltip("Another action button used that is same as teleport activation button")]
    public InputActionProperty m_cancelTeleportation;

    // Update is called once per frame
    void Update()
    {
        m_teleportationRay.SetActive(m_cancelTeleportation.action.ReadValue<float>() == 0 && m_teleportActivation.action.ReadValue<float>() > m_teleportTriggerThreshold);
    }
}
