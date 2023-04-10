using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UiManager : MonoBehaviour
{
    public InputActionReference m_inputToggleAnimalDex;
    public GameObject m_animalDex;
    public Transform m_playerCamera;
    public AudioSource m_audioSourceOpen;
    public AudioSource m_audioSourceClose;

    private bool isAnimalDexOpen;

    private void Start()
    {
        m_animalDex.SetActive(false);
        m_inputToggleAnimalDex.action.performed += ToggleAnimalDex;
    }

    private void ToggleAnimalDex(InputAction.CallbackContext obj)
    {
        ToggleAnimalDex();
    }

    public void ToggleAnimalDex()
    {
        isAnimalDexOpen = !isAnimalDexOpen;
        
        if (isAnimalDexOpen && this != null)
        {
            transform.position = m_playerCamera.position;
            Vector3 newEulerAngles = transform.eulerAngles;
            newEulerAngles.y = m_playerCamera.eulerAngles.y;
            transform.eulerAngles = newEulerAngles;
        }

        m_animalDex.SetActive(isAnimalDexOpen);
        (isAnimalDexOpen ? m_audioSourceOpen : m_audioSourceClose).Play();
    }
}
