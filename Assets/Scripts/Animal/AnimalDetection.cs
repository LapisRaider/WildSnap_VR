using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDetection : MonoBehaviour
{
    public float detectionRadius;
    public Transform m_currentAppleTransform;
    private FoodManager m_foodManager;
    private Transform m_playerTransform;

    private void Awake()
    {
        m_foodManager = GameObject.Find("FoodManager").GetComponent<FoodManager>();
        //m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public bool PlayerInRange()
    {
        return Vector3.SqrMagnitude(transform.position - m_playerTransform.position) < detectionRadius * detectionRadius;
    }

    public bool AppleInRange()
    {
        return m_foodManager.AppleInRange(this);
    }

    public Vector3 GetApplePosition()
    {
        return m_currentAppleTransform.position;
    }


    public void DestroyApple()
    {
        m_foodManager.DestroyApple(this);
    }

}
