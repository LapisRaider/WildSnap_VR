using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [Header("Interface objects")]
    public TextMeshPro m_speechText;
    public TutorialControllerManager m_tutorialController;


    [Header("Tutorial 1")]

    [Header("Tutorial 2")]
    public InputActionReference m_moveJoystick = null;


    private int m_currState = 0;
    private bool m_isPointing = false; //whether player is pointing or not on obj
    private bool m_isClicked = false;

    delegate void TutorialCallbacks();
    List<TutorialCallbacks> m_tutorialFunctions = new List<TutorialCallbacks>();

    // Start is called before the first frame update
    void Awake()
    {
        m_tutorialFunctions.Add(Tutorial_1);
        m_tutorialFunctions.Add(Tutorial_2);
        m_tutorialFunctions.Add(Tutorial_3);
        m_tutorialFunctions.Add(Tutorial_4);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currState < m_tutorialFunctions.Count)
            m_tutorialFunctions[m_currState]();
    }

    // state 1, teach basic interaction via hovering
    void Tutorial_1()
    {
        //play idle animation, waits for player to point at professor with right controller
        if (!m_isPointing)
        {
            m_speechText.text = "Start tutorial by pointing at me with your right controller!";
            m_tutorialController.ShowSideTriggerTutorial(false);
            return;
        }
            
        //once pointed, change the text in UI to ask player to click at them
        m_speechText.text = "Talk to me by pressing the side trigger button!";
        m_tutorialController.ShowSideTriggerTutorial(true);

        //once clicked, go next stage
        if (m_isClicked)
        {
            ++m_currState;
            m_isClicked = false;
            m_tutorialController.ShowSideTriggerTutorial(false);
        }
    }

    //teach player to move via locomotion
    void Tutorial_2()
    {
        m_speechText.text = "Great job! Now try moving with your joystick";
        m_tutorialController.ShowJoyStick(true);

        Vector2 controllerOffset = m_moveJoystick.action.ReadValue<Vector2>();
        if (controllerOffset.sqrMagnitude > 0.0f)
        {
            m_tutorialController.ShowJoyStick(false);
            ++m_currState;
        }
    }

    // teleporting
    void Tutorial_3()
    {
        m_speechText.text = "Nice! Now let's try teleporting \n Hold the back trigger, point, and let go";
        m_tutorialController.ShowBackTriggerTutorial(true);


        //once teleported
        //change text to move to the farmhouse and make the npc walk there
        m_speechText.text = "That's great! Meet me at the farmhouse across the bridge!";
        
    }

    void Tutorial_4()
    {


    }

    public void SetPointing(bool isPointing)
    {
        m_isPointing = isPointing;
    }

    public void Clicked(bool isClicked)
    {
        m_isClicked = isClicked;
    }
}
