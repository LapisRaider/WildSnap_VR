using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalDetection : MonoBehaviour
{
    public float detectionRadius;
    public Transform m_currentAppleTransform;
    private FoodManager m_foodManager;
    private Transform m_playerTransform;
    private NavMeshAgent m_navMeshAgent;
    private NavMeshPath path;

    private void Awake()
    {
        m_foodManager = GameObject.Find("FoodManager").GetComponent<FoodManager>();
        //m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    public bool PlayerInRange()
    {
        return Vector3.SqrMagnitude(transform.position - m_playerTransform.position) < detectionRadius * detectionRadius;
    }

    public bool AppleInRange()
    {
        if (m_foodManager.AppleInRange(this))
        {
            m_navMeshAgent.CalculatePath(m_currentAppleTransform.position, path);
            if (path.status != NavMeshPathStatus.PathComplete) {
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
