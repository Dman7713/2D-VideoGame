using UnityEngine;

public class EnemyPlatformer : MonoBehaviour
{
    public float moveSpeed = 2f;              // Speed at which the enemy moves
    public float jumpForce = 5f;              // Force applied when jumping
    public float detectionRange = 5f;         // Range at which the enemy will detect the player
    public Transform player;                   // Reference to the player object

    private Rigidbody2D rb;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) < detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Move();
        }

        // Check for obstacles
        if (IsObstacleAhead())
        {
            Jump();
        }
    }

    void Move()
    {
        // Move the enemy to the right
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    void ChasePlayer()
    {
        // Move towards the player
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            isGrounded = false; // Set grounded to false until the enemy lands again
        }
    }

    private bool IsObstacleAhead()
    {
        // Cast a ray forward to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0.5f);
        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the enemy is grounded
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Set grounded to true when touching the ground
        }
    }

    private void OnDrawGizmos()
    {
        // Draw ray for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 0.5f);
    }
}
