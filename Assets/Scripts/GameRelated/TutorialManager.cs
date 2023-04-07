using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TutorialManager : MonoBehaviour
{
    [Header("Interface objects")]
    public TextMeshPro m_speechText;
    public TutorialControllerManager m_tutorialController;
    public TutorialHuman m_tutorialHuman;

    [Header("Dialogue settings")]
    private string[][] TUTORIAL_DIALOGUES = new string[][]
    {
        new string[] {"If you would like a tutorial,\n point your right controller at me and press the side trigger button!" } ,
        new string[] {"Great job! If you ever want to interact with any other object, just do what u just did!", "Now try moving with your joystick" },
        new string[] {"Nice! Now let's try teleporting \n Hold the back trigger, point, and let go" },
        new string[] {"That's great! Meet me at the farmhouse across the bridge!", "See you!" },
        new string[] {"You made it! Let's try to take a photo.", "Your camera is on your left hand.",  "Try taking a photo of doggo here by clicking the back trigger!" },
        new string[] {"Great work! You can also zoom in and out using the left joystick", "Now try opening the animal dex, by clicking the Y button on the left controller" },
        new string[] {"You can view all the photos you have taken here",
            "TODO, INSERT HERE HOW TO VIEW PHOTOS",
            "You might notice some animals have various actions",
            "try to photograph them all!",
            "Let me teach you a way to get one of those actions, come to the apple table here."},
        //TODO, talking to players abt the diff state photos, lead them to animal table
        //teach players how to grab apples
        //teach players how to throw apples at animals
        //tell players abt the scoring system

    };

    private int m_currDialogueState = -1;
    private int m_currSentence = 0;

    [Header("Tutorial 1")]

    [Header("Tutorial 2")]
    public InputActionProperty m_moveJoystick;

    [Header("Tutorial 3")]
    public TeleportationProvider m_teleportor = null;
    private bool m_teleported = false;

    [Header("Tutorial 4")]
    public TriggerNotifier m_farmhouseNotifier = null;
    public Transform m_farmLocation = null;
    bool m_playerReachFarmHouse = false;

    [Header("Tutorial 5")]
    public PhotoCapture m_photoCapture = null;

    [Header("Tutorial 6")]
    public InputActionProperty m_openMenuButton;

    [Header("Tutorial 7")]
    public Transform m_appleLocation = null;

    //i need a way to check when player picks up an apple and throw it
    //try throwing to doggo here, and then take a photo of him


    private int m_currState = 0;
    private bool m_isPointing = false; //whether player is pointing or not on obj
    private bool m_isClicked = false;

    delegate void TutorialCallbacks();
    List<TutorialCallbacks> m_tutorialFunctions = new List<TutorialCallbacks>();
    List<TutorialCallbacks> m_initTutorialFunctions = new List<TutorialCallbacks>();

    // Start is called before the first frame update
    void Awake()
    {
        m_tutorialFunctions.Add(Tutorial_1);
        m_tutorialFunctions.Add(Tutorial_2);
        m_tutorialFunctions.Add(Tutorial_3);
        m_tutorialFunctions.Add(Tutorial_4);
        m_tutorialFunctions.Add(Tutorial_5);
        m_tutorialFunctions.Add(Tutorial_6);


        m_initTutorialFunctions.Add(Start_Tutorial_1);
        m_initTutorialFunctions.Add(Start_Tutorial_2);
        m_initTutorialFunctions.Add(Start_Tutorial_3);
        m_initTutorialFunctions.Add(Start_Tutorial_4);
        m_initTutorialFunctions.Add(Start_Tutorial_5);
        m_initTutorialFunctions.Add(Start_Tutorial_6);
    }

    private void Start()
    {
        Start_Tutorial_1();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currState < m_tutorialFunctions.Count)
            m_tutorialFunctions[m_currState]();
    }

    void NextState()
    {
        ++m_currState;

        if (m_currState < m_initTutorialFunctions.Count)
            m_initTutorialFunctions[m_currState]();
    }

    // state 1, teach basic interaction via hovering
    #region TUTORIAL 1 - basic interactions
    void Start_Tutorial_1()
    {
        StartNextDialogue();
        m_tutorialController.ShowSideTriggerTutorial(true);

        //play idle animation
    }

    void Tutorial_1()
    {
        //once clicked, go next stage
        if (m_isClicked && m_isPointing)
        {
            Exit_Tutorial_1();
        }
    }

    void Exit_Tutorial_1()
    {
        m_isClicked = false;
        m_tutorialController.ShowSideTriggerTutorial(false);

        NextState();
    }
    #endregion

    //teach player to move via locomotion
    #region TUTORIAL 2 - move via locomotion
    void Start_Tutorial_2()
    {
        StartNextDialogue();
        m_tutorialController.ShowJoyStick(true);
    }

    void Tutorial_2()
    {
        Vector2 controllerOffset = m_moveJoystick.action.ReadValue<Vector2>();
        if (controllerOffset.sqrMagnitude > 0.0f)
        {
            Exit_Tutorial_2();
        }
    }

    void Exit_Tutorial_2()
    {
        m_tutorialController.ShowJoyStick(false);
        NextState();
    }
    #endregion

    #region TUTORIAL 3 - teleporting
    void Start_Tutorial_3()
    {
        StartNextDialogue();
        m_teleportor.endLocomotion += TeleportationActivated;
        m_tutorialController.ShowBackTriggerTutorial(true);
    }

    void TeleportationActivated(LocomotionSystem system)
    {
        m_teleported = true;
        End_Tutorial_3();
    }

    void Tutorial_3()
    {
        //nothing here
    }

    void End_Tutorial_3()
    {
        m_teleportor.endLocomotion -= TeleportationActivated;
        m_tutorialController.ShowBackTriggerTutorial(false);

        NextState();
    }
    #endregion

    #region TUTORIAL 4 - Ask player to get to the farm house
    void Start_Tutorial_4()
    {
        StartNextDialogue();
        m_farmhouseNotifier.onTriggerCallback += PlayerAtFarmHouse;
    }

    void Tutorial_4()
    {
        //wait for statement to finish first, then start running
        if (m_currSentence > 1)
            m_tutorialHuman.SetDestination(m_farmLocation.position);
    }

    void End_Tutorial_4()
    {
        m_farmhouseNotifier.onTriggerCallback -= PlayerAtFarmHouse;
        NextState();
    }

    void PlayerAtFarmHouse(bool enteredHouse)
    {
        if (enteredHouse)
            End_Tutorial_4();
    }
    #endregion

    #region TUTORIAL 5 - Teach player how to take photos
    void Start_Tutorial_5()
    {
        m_photoCapture.m_photoTakenCallback += PhotoTaken;
        StartNextDialogue();
    }

    void PhotoTaken(AnimalType animal)
    {
        //ends when player has successfully taken a photo
        m_photoCapture.m_photoTakenCallback -= PhotoTaken;
        NextState();
    }

    void Tutorial_5()
    {
        //nothing here
    }
    #endregion

    #region TUTORIAL 6 - Teach players about opening animal dex
    void Start_Tutorial_6()
    {
        StartNextDialogue();
    }

    void Tutorial_6()
    {
        // if press the open dex button move on to next scene
        if (m_openMenuButton.action.ReadValue<float>() > 0)
            Exit_Tutorial_6();
    }

    void Exit_Tutorial_6()
    {
        NextState();
    }
    #endregion

    #region TUTORIAL 7 - Tell players about animal diff state photos, lead them to apple table
    #endregion

    #region TUTORIAL 8 - teach players how to grab apples
    #endregion

    #region TUTORIAL 9 - teach players how to throw apples at animals
    #endregion

    #region TUTORIAL 10 - Tell players about the scoring system, END
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

    #region DIALOGUE
    void StartNextDialogue()
    {
        ++m_currDialogueState;
        m_currSentence = 0;

        StopAllCoroutines();
        StartCoroutine(TypeDialogue());
    }

    IEnumerator TypeDialogue()
    {
        //TODO: start human talking interaction

        m_speechText.text = "";
        foreach (char letter in TUTORIAL_DIALOGUES[m_currDialogueState][m_currSentence].ToCharArray())
        {
            m_speechText.text += letter;
            yield return null;
        }

        ++m_currSentence;
        if (m_currSentence < TUTORIAL_DIALOGUES[m_currDialogueState].Length)
        {
            yield return new WaitForSeconds(1);

            StartCoroutine(TypeDialogue()); // type the next sentence in the dialogue
        }
        else
        {
            //TODO: human stop talking animation
        }
    }
    #endregion
}
