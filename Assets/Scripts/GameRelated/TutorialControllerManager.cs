using UnityEngine;

public class TutorialControllerManager : MonoBehaviour
{
    [Header("Helper Text Objs")]
    public GameObject m_sideTriggerButton;
    public GameObject m_backTriggerButton;
    public GameObject m_joyStick;

    [Header("Current animation state")]
    bool m_showSideTrigger = false;
    float m_sideTriggerOffset = 1.0f;

    bool m_showBackTrigger = false;
    float m_backTriggerOffset = 1.0f;

    bool m_showJoyStick = false;
    float m_joyStickOffset = -1.0f;


    private Animator m_animator;

    // Start is called before the first frame update
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_sideTriggerButton.SetActive(false);
        m_backTriggerButton.SetActive(false);
        m_joyStick.SetActive(false);
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
            m_joyStickOffset = Mathf.PingPong(Time.time, 1.0f);
            m_animator.SetFloat("JoyY", m_joyStickOffset);
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
}
