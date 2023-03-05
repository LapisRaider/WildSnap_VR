using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty m_pinchAnimationAction;
    public InputActionProperty m_gripAnimationAction;

    private Animator m_handAnimator;

    void Awake()
    {
        m_handAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //use float to tell how much the button has been pressed
        float triggerValue = m_pinchAnimationAction.action.ReadValue<float>();
        m_handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = m_gripAnimationAction.action.ReadValue<float>();
        m_handAnimator.SetFloat("Grip", gripValue);
    }
}
