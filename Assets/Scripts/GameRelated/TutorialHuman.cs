using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class TutorialHuman : MonoBehaviour
{
    public ThirdPersonCharacter m_character;
    public NavMeshAgent m_agent;

    [HideInInspector]
    public bool m_isDestinationReached = false;

    // Start is called before the first frame update
    void Start()
    {
        m_agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_agent.remainingDistance > m_agent.stoppingDistance)
        {
            m_character.Move(m_agent.desiredVelocity, false, false);
        }
        else
        {
            //character not moving
            m_character.Move(Vector3.zero, false, false);
            m_isDestinationReached = true;
            //TODO rotate to face the player
        }
    }

    public void SetDestination(Vector3 pos)
    {
        m_agent.SetDestination(pos);
        m_isDestinationReached = false;
    }
}
