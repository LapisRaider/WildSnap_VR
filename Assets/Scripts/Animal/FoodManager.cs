using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public GameObject applePrefab;
    public float appleSpawnCooldown;

    List<Transform> m_appleTransforms = new List<Transform>();
    List<Vector3> m_initialPositions = new List<Vector3>();
    List<Quaternion> m_initialRotations = new List<Quaternion>();

    public delegate void OnAppleEaten();
    public OnAppleEaten onAppleEatenCallback;

    private const float THRESHOLD_DIST = 0.5f;

    private void Awake()
    {
        GameObject[] apples = GameObject.FindGameObjectsWithTag("Apple");
        foreach (GameObject apple in apples)
        {
            m_appleTransforms.Add(apple.transform);
            m_initialPositions.Add(apple.transform.position);
            m_initialRotations.Add(apple.transform.rotation);
        }
    }

    public bool AppleInRange(AnimalDetection animalDetection)
    {
        for (int i = 0; i < m_appleTransforms.Count; i++)
        {
            Transform apple = m_appleTransforms[i];

            if (Vector3.SqrMagnitude(apple.position - m_initialPositions[i]) < THRESHOLD_DIST * THRESHOLD_DIST)
            {
                continue;
            }

            if (Vector3.SqrMagnitude(animalDetection.transform.position - apple.position) < AnimalDetection.detectionRadius * AnimalDetection.detectionRadius)
            {
                animalDetection.m_currentAppleTransform = apple;
                return true;
            }
        }

        return false;
    }

    public void DestroyApple(AnimalDetection animalDetection)
    {
        int appleIndex = m_appleTransforms.IndexOf(animalDetection.m_currentAppleTransform);
        if (appleIndex == -1) return;

        Vector3 newApplePosition = m_initialPositions[appleIndex];
        Quaternion newAppleRotation = m_initialRotations[appleIndex];
        m_appleTransforms.RemoveAt(appleIndex);
        m_initialPositions.RemoveAt(appleIndex);
        m_initialRotations.RemoveAt(appleIndex);
        Destroy(animalDetection.m_currentAppleTransform.gameObject);
        animalDetection.m_currentAppleTransform = null;

        StartCoroutine(CreateApple(newApplePosition, newAppleRotation));

        if (onAppleEatenCallback != null)
            onAppleEatenCallback.Invoke();
    }

    public IEnumerator CreateApple(Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(appleSpawnCooldown);
        
        GameObject newApple = Instantiate(applePrefab, position, rotation);
        m_appleTransforms.Add(newApple.transform);
        m_initialPositions.Add(position);
        m_initialRotations.Add(rotation);
    }
}
