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
    public TutorialControllerManager m_cameraTutorialController;
    public float m_sentencePauseTime = 2.0f;

    public TutorialHuman m_tutorialHuman;

    [Header("Dialogue settings")]
    private string[][] TUTORIAL_DIALOGUES = new string[][]
    {
        new string[] {"If you would like a tutorial,\n point your right controller at me and press the side trigger button!" } ,
        new string[] {"Great job! If you ever want to interact with any other object, just do what u just did!", "Now try moving with your joystick" },
        new string[] {"Nice! Now let's try teleporting \n Hold the back trigger, point, and let go" },
        new string[] {"That's great! Meet me across the bridge!", "See you!", "" },
        new string[] {"You made it! Let's try to take a photo.", "Your camera is on your left hand.",  "Try taking a photo of Baxter the dog here by clicking the back trigger!" },
        new string[] {"Great work! You can also zoom in and out using the left joystick", "Now try opening the animal dex, by clicking the Y button on the left controller" },
        new string[] {"You can view all the photos you have taken here",
            "With your right controller, just point and click on the icons in the animal dex",
            "You might notice some animals have various actions you can photograph.",
            "Let me teach you a way to get one of those actions, come to the apple table here.",
        ""},
        new string[] { "Grab an apple!",
            "With your right controller, try picking it up with the side trigger button",},
        new string[] { "Nice! Now try feeding an animal with it. ",
            "You can toss the apple to an animal, or try feeding Baxter here"
        },
        new string[] { "Great work! Try to photograph as many new animals and actions as you can!",
            "Fill up that animal dex of yours.",
            "Each photograph will also be graded based on how well you took that photo!",
            "You'll also get bonus points if there are multiple animals in the photo.",
            "That's all from me! Have fun!"
        }
    };

    private int m_currSentence = 0;

    [Header("Tutorial 1")]
    public Transform m_tutorialControllerPos_1 = null;

    [Header("Tutorial 2")]
    public InputActionProperty m_moveJoystick;

    [Header("Tutorial 3")]
    public TeleportationProvider m_teleportor = null;
    public Transform m_tutorialControllerPos_3 = null;

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

    [Header("Tutorial 8")]
    public XRDirectInteractor m_directInteractor;
    public Transform m_controllerPos_8;

    [Header("Tutorial 9")]
    public FoodManager m_foodManager = null;

    [Header("Debugging state")]
    public int m_currState = 0;
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
        m_tutorialFunctions.Add(Tutorial_7);
        m_tutorialFunctions.Add(Tutorial_8);
        m_tutorialFunctions.Add(Tutorial_9);
        m_tutorialFunctions.Add(Tutorial_10);

        m_initTutorialFunctions.Add(Start_Tutorial_1);
        m_initTutorialFunctions.Add(Start_Tutorial_2);
        m_initTutorialFunctions.Add(Start_Tutorial_3);
        m_initTutorialFunctions.Add(Start_Tutorial_4);
        m_initTutorialFunctions.Add(Start_Tutorial_5);
        m_initTutorialFunctions.Add(Start_Tutorial_6);
        m_initTutorialFunctions.Add(Start_Tutorial_7);
        m_initTutorialFunctions.Add(Start_Tutorial_8);
        m_initTutorialFunctions.Add(Start_Tutorial_9);
        m_initTutorialFunctions.Add(Start_Tutorial_10);
    }

    private void Start()
    {
        m_cameraTutorialController.gameObject.SetActive(false);

        m_initTutorialFunctions[m_currState]();
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
        m_tutorialController.ShowInteractionRay(true);

        SetObjectPosRot(m_tutorialController.gameObject.transform, m_tutorialControllerPos_1);
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
        m_tutorialController.ShowInteractionRay(false);

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
        m_teleportor.endLocomotion += End_Tutorial_3;
        m_tutorialController.ShowBackTriggerTutorial(true);
        m_tutorialController.ShowTeleportationRay(true);

        SetObjectPosRot(m_tutorialController.gameObject.transform, m_tutorialControllerPos_3);
    }

    void End_Tutorial_3(LocomotionSystem system)
    {
        m_teleportor.endLocomotion -= End_Tutorial_3;
        m_tutorialController.ShowBackTriggerTutorial(false);
        m_tutorialController.ShowTeleportationRay(false);

        m_tutorialController.gameObject.SetActive(false);

        NextState();
    }

    void Tutorial_3()
    {
        //nothing here
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
        if (m_currSentence == TUTORIAL_DIALOGUES[m_currState].Length)
        {
            ++m_currSentence;//hack to run this portion once
            m_speechText.gameObject.SetActive(false);
            m_tutorialHuman.SetDestination(m_farmLocation.position);
        }
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
        m_photoCapture.m_photoTakenCallback += Exit_Tutorial_5;
        StartNextDialogue();

        m_cameraTutorialController.gameObject.SetActive(true);
        m_cameraTutorialController.ShowBackTriggerTutorial(true);
        m_speechText.gameObject.SetActive(true);
    }

    void Exit_Tutorial_5(AnimalType animal)
    {
        //ends when player has successfully taken a photo
        m_photoCapture.m_photoTakenCallback -= Exit_Tutorial_5;
        m_cameraTutorialController.ShowBackTriggerTutorial(false);

        NextState();
    }

    void Tutorial_5()
    {
        //nothing here
    }
    #endregion

    #region TUTORIAL 6 - Teach players about opening animal dex and zoom in and out camera
    void Start_Tutorial_6()
    {
        StartNextDialogue();

        m_cameraTutorialController.ShowJoyStick(true);
        m_cameraTutorialController.ShowYButton(true);
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
        m_cameraTutorialController.ShowJoyStick(false);
        m_cameraTutorialController.ShowYButton(false);

        m_cameraTutorialController.gameObject.SetActive(false);
    }
    #endregion

    #region TUTORIAL 7 - Tell players about animal diff state photos, lead them to apple table
    void Start_Tutorial_7()
    {
        StartNextDialogue();
    }

    void Tutorial_7()
    {
        //once finish speech
        if (m_currSentence == TUTORIAL_DIALOGUES[6].Length)
        {
            ++m_currSentence;
            m_tutorialHuman.SetDestination(m_appleLocation.position);
        }
        else if (m_currSentence > TUTORIAL_DIALOGUES[6].Length && m_tutorialHuman.m_isDestinationReached)
        {
            NextState();
        }

    }
    #endregion


    #region TUTORIAL 8 - teach players how to grab apples
    void Start_Tutorial_8()
    {
        StartNextDialogue();
        m_directInteractor.selectEntered.AddListener(Exit_Tutorial_8);

        m_tutorialController.gameObject.SetActive(true);
        m_tutorialController.ShowSideTriggerTutorial(true);
        m_tutorialController.ShowInteractionRay(true);
        SetObjectPosRot(m_tutorialController.transform, m_controllerPos_8);
    }

    void Tutorial_8()
    {
        return;
    }

    void Exit_Tutorial_8(SelectEnterEventArgs args)
    {
        XRBaseInteractable interactable = (XRBaseInteractable)args.interactableObject;
        if (interactable.tag != "Apple")
            return;

        NextState();
        m_directInteractor.selectEntered.RemoveListener(Exit_Tutorial_8);
        m_tutorialController.gameObject.SetActive(false);
    }
    #endregion

    #region TUTORIAL 9 - teach players how to feed apples to animals
    void Start_Tutorial_9()
    {
        StartNextDialogue();
        m_foodManager.onAppleEatenCallback += End_Tutorial_9;
    }

    void Tutorial_9()
    {
        return;
    }

    void End_Tutorial_9()
    {
        m_foodManager.onAppleEatenCallback -= End_Tutorial_9;
        NextState();
    }
    #endregion

    #region TUTORIAL 10 - Tell players about the scoring system, END
    void Start_Tutorial_10()
    {
        StartNextDialogue();
    }

    void Tutorial_10()
    {
        //TODO: should wait for speech to finish
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

    #region HELPER FUNCTIONS
    void SetObjectPosRot(Transform obj, Transform newTransform)
    {
        obj.position = newTransform.position;
        obj.rotation = newTransform.rotation;
    }
    #endregion

    #region DIALOGUE
    void StartNextDialogue()
    {
        //++m_currDialogueState;
        m_currSentence = 0;
        m_tutorialHuman.Talking(true);

        StopAllCoroutines();
        StartCoroutine(TypeDialogue());
    }

    IEnumerator TypeDialogue()
    {
        m_speechText.text = "";
        foreach (char letter in TUTORIAL_DIALOGUES[m_currState][m_currSentence].ToCharArray())
        {
            m_speechText.text += letter;
            yield return null;
        }

        ++m_currSentence;
        if (m_currSentence < TUTORIAL_DIALOGUES[m_currState].Length)
        {
            yield return new WaitForSeconds(m_sentencePauseTime);

            StartCoroutine(TypeDialogue()); // type the next sentence in the dialogue
        }
        else
        {
            m_tutorialHuman.Talking(false);
        }
    }
    #endregion
}
