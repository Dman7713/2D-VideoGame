using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float horizontalInput;
    Rigidbody2D rb;
    Animator animator; // Reference to the Animator

    [SerializeField]
    float moveSpeed = 5f;
    [SerializeField]
    bool isFacingRight = true;
    [SerializeField]
    float jumpPower = 4f;
    bool isJumping = false;
    [SerializeField]
    float coyoteTime = 0.2f; // Adjusted to a more reasonable value
    bool canJump = true;
    float timer = 0;

    [Header("WallMovement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding;
    bool grounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // Check grounded status
        if (grounded)
        {
            canJump = true; // Reset canJump when grounded
            timer = 0; // Reset timer when on ground
        }
        else
        {
            timer += Time.deltaTime; // Increase timer if not grounded
        }

        FlipSprite();

        // Handle jumping
        if (Input.GetButtonDown("Jump") && (grounded || timer < coyoteTime))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isJumping = true;
            grounded = false;
            canJump = false; // No more jumping until grounded again
        }

        // Update animation parameters
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    void FlipSprite()
    {
        // Check the mouse position relative to the player
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x < transform.position.x && isFacingRight)
        {
            // Flip to face left
            isFacingRight = false;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
        else if (mousePosition.x > transform.position.x && !isFacingRight)
        {
            // Flip to face right
            isFacingRight = true;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6) // Ensure this matches your ground layer
        {
            grounded = true; // Set grounded to true
            isJumping = false; // Reset jumping state when touching ground
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6) // Ensure this matches your ground layer
        {
            grounded = false; // Set grounded to false when leaving ground
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6) // Ensure this matches your ground layer
        {
            // No need to reset timer here, handled in Update()
        }
    }

    void UpdateAnimations()
    {
        // Set Speed and IsJumping parameters for the Animator
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput)); // Use Mathf.Abs to get the absolute value for speed
        animator.SetBool("IsJumping", !grounded); // Set IsJumping to true if not grounded
    }
}
