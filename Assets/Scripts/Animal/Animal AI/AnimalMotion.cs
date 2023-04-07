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
    Eating,
    WatchingPlayer
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
        m_animator.logWarnings = false;
    }

    public void SetIdle()
    {
        m_animator.SetBool("isWalking", false);
        m_animator.SetBool("isAttacking", false);
        m_navMeshAgent.isStopped = true;
        m_currentState = MotionState.Idle;
    }

    public void WalkToPoint(Vector3 destination)
    {
        m_animator.SetBool("isWalking", true);
        m_navMeshAgent.SetDestination(destination);
        m_navMeshAgent.isStopped = false;
        m_currentState = MotionState.Walking;
    }

    public void WalkToApple(Vector3 destination)
    {
        m_animator.SetBool("isWalking", true);
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

    public bool ReachedDestination(float distanceFromDestination=0.5f)
    {
        return m_navMeshAgent.remainingDistance < distanceFromDestination;
    }


    public MotionState GetMotion()
    {
        return m_currentState;
    }

    public void StartEating()
    {
        m_animator.SetBool("isWalking", false);
        m_navMeshAgent.isStopped = true;
        m_currentState = MotionState.Eating;
    }
    public void EndEating()
    {
        m_navMeshAgent.isStopped = false;
        m_currentState = MotionState.Idle;
    }

    public void StartWatchingPlayer()
    {
        m_animator.SetBool("isAttacking", true);
        m_navMeshAgent.isStopped = true;
        m_currentState = MotionState.WatchingPlayer;
    }
    public void EndWatchingPlayer()
    {
        m_animator.SetBool("isAttacking", false);
        m_navMeshAgent.isStopped = false;
        m_currentState = MotionState.Idle;
    }
}
