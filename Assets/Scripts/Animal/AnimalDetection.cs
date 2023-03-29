using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDetection : MonoBehaviour
{
    public float detectionRadius;
    public Transform m_currentAppleTransform;
    private FoodManager m_foodManager;
    private Transform m_playerTransform;
    private UnityEngine.AI.NavMeshAgent m_navMeshAgent;
    private UnityEngine.AI.NavMeshPath path;

    private void Awake()
    {
        m_foodManager = GameObject.Find("FoodManager").GetComponent<FoodManager>();
        //m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        path = new UnityEngine.AI.NavMeshPath();
    }

    public bool PlayerInRange()
    {
        return Vector3.SqrMagnitude(transform.position - m_playerTransform.position) < detectionRadius * detectionRadius;
    }

    public bool AppleInRange()
    {
        if (m_foodManager.AppleInRange(this))
        {
            print("apple detected");
            m_navMeshAgent.CalculatePath(m_currentAppleTransform.position, path);
            if (path.status != UnityEngine.AI.NavMeshPathStatus.PathComplete) {
                return false;
            }
            else {
                return true;
            }
        }
        return false;
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
