using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MotionState
{
    Idle,
    Walking,
    Running,
    Sleep,
    WalkingApple,
    Eating
}

public class AnimalMotion : MonoBehaviour
{
    private NavMeshAgent m_navMeshAgent;
    private Animator m_animator;
    private MotionState m_currentState;
    // Start is called before the first frame update
    private void Awake()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
    }

    public void SetIdle()
    {
        m_animator.SetInteger("ID", 0);
        m_navMeshAgent.isStopped = true;
        m_currentState = MotionState.Idle;
    }

    public void WalkToPoint(Vector3 destination)
    {
        m_animator.SetInteger("ID", 1);
        m_navMeshAgent.SetDestination(destination);
        m_navMeshAgent.isStopped = false;
        m_currentState = MotionState.Walking;
    }

    public void WalkToApple(Vector3 destination)
    {
        m_animator.SetInteger("ID", 1);
        m_navMeshAgent.SetDestination(destination);
        m_navMeshAgent.isStopped = false;
        m_currentState = MotionState.WalkingApple;
    }

    public void SetDestination(Vector3 destination)
    {
        m_navMeshAgent.SetDestination(destination);
    }

    public void SetSleep()
    {
        m_animator.SetTrigger("Sleeping");
        m_navMeshAgent.isStopped = true;
        m_currentState = MotionState.Sleep;
    }

    public bool ReachedDestination()
    {
        return m_navMeshAgent.remainingDistance < 2.0f;
    }

    public bool ReachedPlayer()
    {
        return m_navMeshAgent.remainingDistance < 10.0f;
    }


    public MotionState GetMotion()
    {
        return m_currentState;
    }

    public void StartEating()
    {
        m_animator.SetInteger("ID", 0);
        m_navMeshAgent.isStopped = true;
        m_currentState = MotionState.Eating;
    }
    public void EndEating()
    {
        m_animator.SetInteger("ID", 1);
        m_navMeshAgent.isStopped = false;
        m_currentState = MotionState.Idle;
    }
}
