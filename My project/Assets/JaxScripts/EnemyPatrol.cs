using UnityEngine;

public class EnemyWander : MonoBehaviour
{
    public float moveSpeed = 2f;          // Speed at which the enemy moves
    public float wanderTime = 2f;          // Time to wander before changing direction
    public Vector2 wanderRange = new Vector2(5f, 5f); // Range within which the enemy will wander

    public Transform player;               // Reference to the player object
    public float detectionRange = 5f;      // Range at which the enemy will detect the player

    private Vector2 targetPosition;
    private float timer;
    private bool chasing = false;

    void Start()
    {
        SetRandomTargetPosition();
    }

    void Update()
    {
        // Check if the player is within detection range
        if (Vector2.Distance(transform.position, player.position) < detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Wander();
        }
    }

    void Wander()
    {
        MoveTowardsTarget();

        timer += Time.deltaTime;
        if (timer >= wanderTime)
        {
            SetRandomTargetPosition();
            timer = 0; // Reset timer
        }
    }

    void ChasePlayer()
    {
        // Move towards the player
        targetPosition = player.position;
        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        // Move the enemy towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void SetRandomTargetPosition()
    {
        // Generate a random position within the defined wander range
        float randomX = Random.Range(-wanderRange.x, wanderRange.x);
        float randomY = Random.Range(-wanderRange.y, wanderRange.y);
        targetPosition = new Vector2(transform.position.x + randomX, transform.position.y + randomY);
    }
}
