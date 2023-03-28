using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UiManager : MonoBehaviour
{
    public InputActionReference m_inputToggleAnimalDex;
    public GameObject m_animalDex;

    private bool isAnimalDexOpen;

    private void Start()
    {
        m_animalDex.SetActive(false);
        m_inputToggleAnimalDex.action.performed += ToggleAnimalDex;
    }

    private void ToggleAnimalDex(InputAction.CallbackContext obj)
    {
        print("hi");
        isAnimalDexOpen = !isAnimalDexOpen;
        m_animalDex.SetActive(isAnimalDexOpen);
    }
}
