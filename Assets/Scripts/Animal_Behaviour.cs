using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Animal_Behaviour : MonoBehaviour
{
    public AnimalType m_animalType;
    private BoxCollider m_boxCollider;
    private NavMeshAgent m_navMeshAgent;
    private Animator m_animator;
    private Dictionary<string, AnimalState> m_stateMap;
    // Start is called before the first frame update
    void Awake()
    {
        m_boxCollider = transform.GetComponentInChildren<BoxCollider>();
        m_navMeshAgent = transform.GetComponent<NavMeshAgent>();
        m_animator = transform.GetComponent<Animator>();
        m_stateMap = new Dictionary<string, AnimalState>(){
            { "IsWalking", AnimalState.Walking},
            {"IsStanding", AnimalState.Standing},
            {"IsRunning", AnimalState.Running},
            {"IsAttacking", AnimalState.Attacking},
            {"IsDead" , AnimalState.Dead } };
    }

    private void OnTriggerEnter(Collider other)
    {
        // Dysfunctional, probably have to tweak with Wander Script cause the nav mesh are conflicting
        /*
        if (other.CompareTag("Apple"))
        {
            Debug.Log("Found Apple");
            m_animator.SetBool("IsWalking", true);
            m_navMeshAgent.SetDestination(other.transform.position);
            if (m_navMeshAgent.remainingDistance < 0.1f)
            {
                m_navMeshAgent.isStopped = true;
                m_animator.SetBool("IsWalking", false);
                Destroy(other.gameObject);
            }
        }
        */
    }

    public AnimalState GetAnimalState()
    {
        foreach (var state in m_stateMap)
        {
            if (m_animator.GetBool(state.Key))
            {
                return state.Value;
            }
        }

        return AnimalState.Idling;
    }
}
