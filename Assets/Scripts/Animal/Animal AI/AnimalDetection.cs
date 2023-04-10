using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalDetection : MonoBehaviour
{
    public bool isAggressive = false;
    public bool isScared = false;
    public float fleeingCoef = 0.7f;
    public Transform m_currentAppleTransform;
    private FoodManager m_foodManager;
    private Transform m_playerTransform;
    private NavMeshAgent m_navMeshAgent;
    private NavMeshPath path;
    
    public const float detectionRadius = 3f;
    public const float playerDetectionRadius = 8f;

    private void Awake()
    {
        m_foodManager = GameObject.Find("FoodManager").GetComponent<FoodManager>();
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    public bool PlayerInRange()
    {
        if (!isAggressive && !isScared) {
            return false;
        }
        if (Vector3.SqrMagnitude(transform.position - m_playerTransform.position) < playerDetectionRadius * playerDetectionRadius)
        {
            /*
            if (isAggressive) {
                m_navMeshAgent.CalculatePath(m_playerTransform.position, path);
                if (path.status != NavMeshPathStatus.PathComplete) {
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                return true;
            }*/
            return true;

        }
        return false;
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
        if (appleExists()) {
            return m_currentAppleTransform.position;
        }
        else {
            return transform.position;
        }
    }
    public Vector3 GetPlayerPosition()
    {
        return m_playerTransform.position;
    }

    public Vector3 GetAnimalPosition()
    {
        return transform.position;
    }


    public void DestroyApple()
    {
        if (appleExists()) {
            m_foodManager.DestroyApple(this);
        }

    }

    public bool appleExists() {
        return (m_currentAppleTransform != null);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}
