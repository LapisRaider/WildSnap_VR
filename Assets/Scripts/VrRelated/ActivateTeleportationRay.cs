using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.EventSystems;

public class ActivateTeleportationRay : MonoBehaviour
{
    public GameObject m_teleportationRay;

    public InputActionProperty m_teleportActivation;
    public float m_teleportTriggerThreshold = 0.1f;

    public XRRayInteractor m_interactableRay;

    [Tooltip("Another action button used that is same as teleport activation button")]
    public InputActionProperty m_cancelTeleportation;

    // Update is called once per frame
    void Update()
    {
        /** only show teleportation ray when:
         * 1. player holds the teleportation trigger button
         * 2. player is not grabbing any objects (m_cancelTeleportation)
         * 3. the interactable ray that shows up to interact with interactable obj is not shown
        */

        //bool isInteractableRayActive = m_interactableRay.TryGetHitInfo(out Vector3 pos, out Vector3 leftNormal, out int leftNumber, out bool leftValid);
        //bool isOtherActionInputActive = m_cancelTeleportation.action.ReadValue<float>() == 0;
        bool isHittingUi = m_interactableRay.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult, out int raycastEndpointIndex);
        bool isPlayerHoldingTeleport = m_teleportActivation.action.ReadValue<float>() > m_teleportTriggerThreshold;

        m_teleportationRay.SetActive(isPlayerHoldingTeleport && !isHittingUi);
    }
}
