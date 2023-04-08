using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TutorialDogController : MonoBehaviour
{
    private NavMeshAgent m_agent;
    private Animator m_animator;

    public Transform m_humanLookAt;

    // Start is called before the first frame update
    void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_animator.SetBool("isBarking", true);
    }

    public void MoveDog(Vector3 pos)
    {
        m_agent.SetDestination(pos);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_agent.remainingDistance <= m_agent.stoppingDistance)
        {
            m_animator.SetBool("isBarking", true);
            m_animator.SetBool("isRunning", false);
            transform.LookAt(m_humanLookAt);
        }
        else
        {
            m_animator.SetBool("isBarking", false);
            m_animator.SetBool("isRunning", true);
        }
    }
}
