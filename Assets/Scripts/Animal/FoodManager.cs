using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    List<Transform> m_appleTransforms = new List<Transform>();

    private void Awake()
    {
        GameObject[] apples = GameObject.FindGameObjectsWithTag("Apple");
        foreach (GameObject apple in apples)
        {
            m_appleTransforms.Add(apple.transform);
        }
    }

    public bool AppleInRange(AnimalDetection animalDetection)
    {
        foreach (Transform apple in m_appleTransforms)
        {
            if (Vector3.SqrMagnitude(animalDetection.transform.position - apple.position) < animalDetection.detectionRadius * animalDetection.detectionRadius)
            {
                animalDetection.m_currentAppleTransform = apple;
                return true;
            }
        }

        return false;
    }

    public void DestroyApple(AnimalDetection animalDetection)
    {
        m_appleTransforms.Remove(animalDetection.m_currentAppleTransform);
        Destroy(animalDetection.m_currentAppleTransform.gameObject);
        animalDetection.m_currentAppleTransform = null;
    }
}
