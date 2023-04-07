using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class TutorialHuman : MonoBehaviour
{
    public ThirdPersonCharacter m_character;
    public NavMeshAgent m_agent;
    public Transform m_player;
    private Animator m_animator;

    [HideInInspector]
    public bool m_isDestinationReached = false;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

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
            transform.LookAt(m_player);
        }
    }

    public void SetDestination(Vector3 pos)
    {
        m_agent.SetDestination(pos);
        m_isDestinationReached = false;
    }

    public void Talking(bool isTalking)
    {
        m_animator.SetBool("Talking", isTalking);
    }
}
