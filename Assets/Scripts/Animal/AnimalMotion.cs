using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MotionState
{
    Idle,
    Walking,
    Running
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
        m_animator.SetBool("IsIdling", true);
        m_navMeshAgent.isStopped = true;
        m_currentState = MotionState.Idle;
    }

    public MotionState GetMotion()
    {
        return m_currentState;
    }
}
