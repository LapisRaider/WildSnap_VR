using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class TutorialManager : MonoBehaviour
{
    [Header("Interface objects")]
    public TextMeshPro m_speechText;
    public TutorialControllerManager m_tutorialController;

    [Header("Dialogue settings")]
    private string[][] TUTORIAL_DIALOGUES = new string[][]
    {
        new string[] {"Start tutorial by pointing at me with your right controller!" } ,
        new string[] {"Talk to me by pressing the side trigger button!" },
        new string[] {"Great job! Now try moving with your joystick" },
        new string[] {"Nice! Now let's try teleporting \n Hold the back trigger, point, and let go" },
        new string[] {"That's great! Meet me at the farmhouse across the bridge!" },
        new string[] {"You made it! Let's try to take a photo.", "Your camera is on your left hand.",  "Try taking a photo of doggo here by clicking the back trigger!" },
        new string[] {"Great work! You can also zoom in and out using the left joystick" },
        new string[] {"Now try opening the animal dex, by clicking the XXXX button" },
    };
    private int m_currDialogueState = 0;
    private int m_currSentence = 0;


    [Header("Tutorial 1")]

    [Header("Tutorial 2")]
    public InputActionReference m_moveJoystick = null;

    [Header("Tutorial 3")]
    public LocomotionProvider m_teleportor = null;
    private bool m_teleported = false;

    [Header("Tutorial 4")]
    //public TriggerNotifier m_farmhouseNotifier = null;
    bool playerReachFarmHouse = false;



    private int m_currState = 0;
    private bool m_isPointing = false; //whether player is pointing or not on obj
    private bool m_isClicked = false;

    private bool m_pauseTutorialUpdate = false;

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
        if (m_pauseTutorialUpdate)
            return;

        if (m_currState < m_tutorialFunctions.Count)
            m_tutorialFunctions[m_currState]();
    }

    void NextState()
    {
        ++m_currState;

    }

    // state 1, teach basic interaction via hovering
    #region TUTORIAL 1 - basic interactions
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
    #endregion

    //teach player to move via locomotion
    #region TUTORIAL 2 - move via locomotion
    void Tutorial_2()
    {
        m_speechText.text = "Great job! Now try moving with your joystick";
        m_tutorialController.ShowJoyStick(true);

        Vector2 controllerOffset = m_moveJoystick.action.ReadValue<Vector2>();
        if (controllerOffset.sqrMagnitude > 0.0f)
        {
            m_tutorialController.ShowJoyStick(false);
            ++m_currState;
            Start_Tutorial_3();
        }
    }
    #endregion

    #region TUTORIAL 3 - teleporting
    void Start_Tutorial_3()
    {
        m_teleportor.endLocomotion += TeleportationActivated;
    }

    void End_Tutorial_3()
    {
        m_teleportor.endLocomotion -= TeleportationActivated;
        m_tutorialController.ShowBackTriggerTutorial(false);
        ++m_currState;
    }

    void TeleportationActivated(LocomotionSystem system)
    {
        m_teleported = true;
    }

    // teleporting
    void Tutorial_3()
    {
        m_speechText.text = "Nice! Now let's try teleporting \n Hold the back trigger, point, and let go";
        m_tutorialController.ShowBackTriggerTutorial(true);

        //change text to move to the farmhouse and make the npc walk there
        if (m_teleported)
        {
            m_speechText.text = "That's great! Meet me at the farmhouse across the bridge!";
            End_Tutorial_3();
            Start_Tutorial_4();
        }  
    }
    #endregion

    #region TUTORIAL 4 - taking photos
    void Start_Tutorial_4()
    {
        m_farmhouseNotifier.onTriggerCallback += PlayerAtFarmHouse;
        m_pauseTutorialUpdate = true;
    }

    void End_Tutorial_4()
    {
        m_farmhouseNotifier.onTriggerCallback -= PlayerAtFarmHouse;
        ++m_currState;
    }

    void PlayerAtFarmHouse(bool enteredHouse)
    {
        playerReachFarmHouse = enteredHouse;

        if (enteredHouse)
        {
            m_pauseTutorialUpdate = true;
            m_speechText.text = "You made it! Let's try to take a photo! ";
        }
    }

    void Tutorial_4()
    {

        //once reach the farmhouse, there should be a bigass collider box here


        //should activate new speech, teaching players to throw apples
        //afterwards teach them to take photos


    }
    #endregion

    #region VR INTERACTIONS
    public void SetPointing(bool isPointing)
    {
        m_isPointing = isPointing;
    }

    public void Clicked(bool isClicked)
    {
        m_isClicked = isClicked;
    }
    #endregion


    //IEnumerator TypeLine()
    //{
    //    //foreach (char letter in )
    //}
}
