using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class TutorialHuman : MonoBehaviour
{
    [Header("Doggo follower")]
    public TutorialDogController m_doggo;

    [Header("Character related")]
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
        if (!m_agent.enabled)
        {
            m_character.Move(Vector3.zero, false, false);
            transform.LookAt(new Vector3(m_player.position.x, transform.position.y, m_player.position.z));
            return;
        }

        if (m_agent.remainingDistance > m_agent.stoppingDistance)
        {
            m_character.Move(m_agent.desiredVelocity, false, false);
        }
        else
        {
            //character not moving
            m_isDestinationReached = true;
            m_agent.enabled = false;
        }
    }

    public void SetDestination(Vector3 pos)
    {
        m_agent.enabled = true;
        m_agent.SetDestination(pos);
        m_isDestinationReached = false;

        m_doggo.MoveDog(pos);

    }

    public void Talking(bool isTalking)
    {
        m_animator.SetBool("Talking", isTalking);
    }
}
