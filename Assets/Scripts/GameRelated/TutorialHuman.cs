using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class TutorialHuman : MonoBehaviour
{
    public ThirdPersonCharacter m_character;
    public NavMeshAgent m_agent;

    // Start is called before the first frame update
    void Start()
    {
        m_agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_agent.remainingDistance > m_agent.stoppingDistance)
            m_character.Move(m_agent.desiredVelocity, false, false);
        else
            m_character.Move(Vector3.zero, false, false);
    }

    public void SetDestination(Vector3 pos)
    {
        m_agent.SetDestination(pos);
    }
}
