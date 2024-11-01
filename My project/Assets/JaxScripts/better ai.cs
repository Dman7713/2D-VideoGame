using UnityEngine;
using UnityEngine.AI;

public class betterai : MonoBehaviour
{
    public float wanderRadius = 10f;   // Radius in which the enemy can wander
    public float wanderInterval = 2f;   // Time interval between movements
    public float detectionRange = 5f;   // Range to detect the player
    public float returnRange = 6f;      // Range to stop chasing
    public float chaseSpeed = 5f;       // Speed while chasing

    private NavMeshAgent agent;         // Reference to the NavMeshAgent
    private Transform player;           // Reference to the player's transform
    private float nextWanderTime;       // Time to wait before wandering again

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Wander();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            ChasePlayer();
        }
        else if (Time.time >= nextWanderTime && distanceToPlayer > returnRange)
        {
            Wander();
        }
    }

    void Wander()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas);
        agent.SetDestination(hit.position);

        nextWanderTime = Time.time + wanderInterval; // Set the next time to wander
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
        agent.speed = chaseSpeed; // Set speed for chasing
    }
}
