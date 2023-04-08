using UnityEngine;

public class TutorialControllerManager : MonoBehaviour
{
    [Header("Helper Text Objs")]
    public GameObject m_sideTriggerButton = null;
    public GameObject m_backTriggerButton = null;
    public GameObject m_joyStick = null;
    public GameObject m_yButton = null;

    [Header("Rays")]
    public GameObject m_teleportationRay = null;
    public GameObject m_interactionRay = null;


    [Header("Current animation state")]
    bool m_showSideTrigger = false;
    float m_sideTriggerOffset = 1.0f;

    bool m_showBackTrigger = false;
    float m_backTriggerOffset = 1.0f;

    bool m_showJoyStick = false;
    float m_joyStickOffset = -1.0f;

    bool m_showYButton = false;
    float m_yButtonOffset = 0.0f;


    private Animator m_animator;

    // Start is called before the first frame update
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        if (m_sideTriggerButton)
            m_sideTriggerButton.SetActive(false);

        if (m_backTriggerButton)
            m_backTriggerButton.SetActive(false);

        if (m_joyStick)
            m_joyStick.SetActive(false);

        if (m_yButton)
            m_yButton.SetActive(false);

        if (m_teleportationRay)
            m_teleportationRay.SetActive(false);

        if (m_interactionRay)
            m_interactionRay.SetActive(false);
    }

    private void Update()
    {
        if (m_showSideTrigger)
        {
            m_sideTriggerOffset = Mathf.PingPong(Time.time, 1.0f);
            m_animator.SetFloat("Grip", m_sideTriggerOffset);
        }

        if (m_showBackTrigger)
        {
            m_backTriggerOffset = Mathf.PingPong(Time.time, 1.0f);
            m_animator.SetFloat("Trigger", m_backTriggerOffset);
        }

        if (m_showJoyStick)
        {
            m_joyStickOffset = Mathf.PingPong(Time.time, 2.0f) - 1.0f;
            m_animator.SetFloat("JoyY", m_joyStickOffset);
        }

        if (m_showYButton)
        {
            m_yButtonOffset = Mathf.PingPong(Time.time, 1.0f);
            m_animator.SetFloat("Button 1", m_yButtonOffset);
        }
    }

    public void ShowSideTriggerTutorial(bool show = true)
    {
        m_sideTriggerButton.SetActive(show);
        m_showSideTrigger = show;

        if (!show)
            m_animator.SetFloat("Grip", 1.0f);
    }

    public void ShowBackTriggerTutorial(bool show = true)
    {
        m_backTriggerButton.SetActive(show);
        m_showBackTrigger = show;

        if (!show)
            m_animator.SetFloat("Trigger", 1.0f);
    }

    public void ShowJoyStick(bool show = true)
    {
        m_joyStick.SetActive(show);
        m_showJoyStick = show;

        if (!show)
            m_animator.SetFloat("JoyY", 0.0f);
    }

    public void ShowYButton(bool show = true)
    {
        m_yButton.SetActive(show);
        m_showYButton = show;

        if (!show)
            m_animator.SetFloat("Button 1", 0.0f);
    }

    public void ShowTeleportationRay(bool show = true)
    {
        m_teleportationRay.SetActive(show);
    }

    public void ShowInteractionRay(bool show = true)
    {
        m_interactionRay.SetActive(show);
    }
}
