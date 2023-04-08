using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Parrot : MonoBehaviour
{
    public float MOVE_TIMEOUT = 5.0f;
    public float flyAwayDistance = 5.0f;
    public float avoidanceRadius = 3.0f;

    private Transform playerTransform;
    private NavMeshAgent agent;
    private Animator animator;
    private AnimalMotion animalMotion;
    private Vector3 initialPos;
    private Vector3 goalPos;
    private bool isAscending; // Whether the bird is ascending or descending

    private bool isRunningAway = false;
    private bool hasFlown = false;
    private float moveTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        animalMotion = GetComponent<AnimalMotion>();
        isAscending = true; // Set bird to ascending initially
        if (playerTransform == null)
        {
            Debug.LogError("Player not found! Make sure the player object has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        Vector3 distanceToPlayer = transform.position - playerTransform.position;

        // If player is close
        if (distanceToPlayer.magnitude < avoidanceRadius && !isRunningAway)
        {
            // Run away by setting goal position to be away from the player
            isRunningAway = true;
            hasFlown = false;
            Vector3 moveDirection = distanceToPlayer.normalized * Random.Range(flyAwayDistance * 0.9f, flyAwayDistance * 1.1f);
            initialPos = transform.localPosition;
            goalPos = moveDirection + transform.localPosition;

            agent.SetDestination(goalPos);
            animator.SetBool("isFlying", true);
            animalMotion.SetFlying();
        }

        if (isRunningAway)
        {
            moveTimer += Time.deltaTime;
            if (moveTimer > MOVE_TIMEOUT || (transform.localPosition - goalPos).magnitude < 0.3f)
            {
                moveTimer = 0f;
                isRunningAway = false;
                animator.SetBool("isFlying", false);
                animalMotion.SetIdling();
            }
        }
    }
}
