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
    private Dictionary<string, AnimalState> m_animStateMap = new Dictionary<string, AnimalState>();


    // Start is called before the first frame update
    void Awake()
    {
        m_boxCollider = transform.GetComponentInChildren<BoxCollider>();
        m_navMeshAgent = transform.GetComponent<NavMeshAgent>();
        m_animator = transform.GetComponent<Animator>();

        AnimalDexEntry dexEntry = AnimalDex.Instance.GetAnimalDexEntry(m_animalType);
        Animal_Info info = dexEntry.m_animalInfo;
        foreach (AnimalAnimationStates stateInfo in info.m_animStateData)
        {
            m_animStateMap.Add(stateInfo.m_animStateName, stateInfo.m_state);
        }
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
        AnimatorClipInfo[] currClipInfo = m_animator.GetCurrentAnimatorClipInfo(0);
        string clipName = currClipInfo[0].clip.name;

        if (!m_animStateMap.ContainsKey(clipName))
        {
            Debug.LogError("Missing state: " + clipName + " check scriptable object for " + m_animalType);
            return AnimalState.Idling;
        }

        return m_animStateMap[clipName];
    }
}

