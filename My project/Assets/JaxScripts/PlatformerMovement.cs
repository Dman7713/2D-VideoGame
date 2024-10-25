using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;  // Player movement speed
    [SerializeField] float jumpSpeed = 2f;  // Player jump force
    [SerializeField] float coyoteTime = 0.2f;  // Allow slight jumping after leaving the ground
    [SerializeField] Animator animator;  // Reference to the Animator
    [SerializeField] Transform spriteTransform;  // The GameObject containing the player's sprite and animator (the one to flip)

    bool grounded = false;  // To check if the player is on the ground
    bool canJump = true;  // To check if the player can jump
    bool facingRight = true;  // To check if the player is facing right
    float timer = 0;  // Coyote time timer

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        Vector2 velocity = rb.velocity;

        // Horizontal movement logic
        velocity.x = moveX * moveSpeed;
        rb.velocity = velocity;

        // Update the animator with the speed (absolute value)
        animator.SetFloat("Speed", Mathf.Abs(moveX));

        // Flip the sprite when moving in the opposite direction
        if (moveX > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveX < 0 && facingRight)
        {
            Flip();
        }

        // Timer for coyote time
        timer += Time.deltaTime;
        if (timer > coyoteTime && !grounded)
        {
            canJump = false;
        }

        // Jump logic
        if (Input.GetButtonDown("Jump") && canJump)
        {
            rb.AddForce(new Vector2(0, 100 * jumpSpeed));
            grounded = false;
            canJump = false;

            // Set animator jumping state
            animator.SetBool("IsJumping", true);
        }

        // Crouch logic
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.SetTrigger("Crouch");
        }

        // Check if the player is not grounded
        if (!grounded)
        {
            animator.SetBool("IsJumping", true); // Set jumping animation if not grounded
        }
        else if (Mathf.Abs(moveX) < 0.1f && grounded)
        {
            animator.SetBool("IsJumping", false);  // Not jumping anymore
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) // Assuming layer 6 is ground
        {
            timer = 0;
            grounded = true;
            canJump = true;

            // Reset jumping animation when grounded
            animator.SetBool("IsJumping", false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            timer = 0;
            grounded = false;
        }
    }

    // Method to flip the sprite only (not the entire player object)
    void Flip()
    {
        // Reverse the direction the player is facing
        facingRight = !facingRight;

        // Multiply the sprite's local scale by -1 to flip only the sprite and animations
        Vector3 scale = spriteTransform.localScale;
        scale.x *= -1;
        spriteTransform.localScale = scale;
    }
}
