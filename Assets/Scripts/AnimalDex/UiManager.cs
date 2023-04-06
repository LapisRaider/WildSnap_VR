using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UiManager : MonoBehaviour
{
    public InputActionReference m_inputToggleAnimalDex;
    public GameObject m_animalDex;
    public Transform m_playerCamera;

    private bool isAnimalDexOpen;

    private void Start()
    {
        m_animalDex.SetActive(false);
        m_inputToggleAnimalDex.action.performed += ToggleAnimalDex;
    }

    private void ToggleAnimalDex(InputAction.CallbackContext obj)
    {
        isAnimalDexOpen = !isAnimalDexOpen;
        
        if (isAnimalDexOpen && this.gameObject.activeInHierarchy)
        {
            transform.position = m_playerCamera.position;
            Vector3 newEulerAngles = transform.eulerAngles;
            newEulerAngles.y = m_playerCamera.eulerAngles.y;
            transform.eulerAngles = newEulerAngles;
        }

        m_animalDex.SetActive(isAnimalDexOpen);
    }
}
