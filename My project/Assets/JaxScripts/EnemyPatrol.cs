using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of points to patrol between
    public float patrolSpeed = 2f;   // Speed while patrolling
    public float chaseSpeed = 5f;    // Speed while chasing
    public float detectionRange = 5f; // Range to detect the player
    public float returnRange = 6f;    // Range to stop chasing

    private int currentPatrolIndex;
    private Transform player;

    void Start()
    {
        currentPatrolIndex = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            ChasePlayer();
        }
        else if (distanceToPlayer > returnRange)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        Transform targetPatrolPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPatrolPoint.position, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPatrolPoint.position) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }
}