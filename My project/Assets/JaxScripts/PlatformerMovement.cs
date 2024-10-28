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

    [Header("Wallcheck")] // checks for any walls touching the player
    public Transform wallCheckPos; 
    public Vector2 wallCheckSize = new Vector2(0.49f, 0.03f);
    public LayerMask wallLayer;

    [Header("WallMovment")] // wallslide basic funtions
    public float wallSlideSpeed = 2;
    bool isWallSlideing;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        var velocity = GetComponent<Rigidbody2D>().velocity;
        velocity.x += moveX * accelleration * Time.deltaTime;
        velocity.x = Mathf.Clamp(velocity.x, -moveSpeed, moveSpeed);
        GetComponent<Rigidbody2D>().velocity = velocity;
        /*if(Input.GetButtonDown("Jump") && grounded)
        {
            GetComponent<Rigidbody2D>().AddForce(
                new Vector2(0, 100 * jumpSpeed));
        }*/
        if (Input.GetButtonDown("Jump"))
        {
            grounded = CheckIfGrounded();
            if (grounded)
            {
                GetComponent<Rigidbody2D>().AddForce(
                new Vector2(0, 100 * jumpSpeed));
            }
            else if (CheckIfLeftWallJump())
            {
                GetComponent<Rigidbody2D>().AddForce(
                new Vector2(100 * wallJumpSpeed, 100 * jumpSpeed));
            }
            else if (CheckIfRightWallJump())
            {
                GetComponent<Rigidbody2D>().AddForce(
                new Vector2(-100 * wallJumpSpeed, 100 * jumpSpeed));
            }
        }

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

    private void OnDrawGizmosSelected()
    {
        // vizuals for wall check
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
    private void processWallSlide()
    {
        // not grounded &  on a wall & movement != 0
        if (!grounded & WallCheck())
        {

        }
    }
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }
    public float accelleration = 1.0f;
    public bool leftWall = false;
    public float wallJumpSpeed = 1.0f;
   

   
    bool CheckIfGrounded()
    {
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position, new Vector2(0, -1), Mathf.Infinity);
        Debug.Log(hit[1].collider.gameObject.name);
        Debug.DrawRay(transform.position, new Vector2(0, -1 * transform.lossyScale.y * 0.6f), Color.red, 0.25f);
        if (hit[1].collider.gameObject.layer == 6 && hit[1].distance < transform.localScale.y * 0.6f)// && hit[1].distance > 0.6f * transform.localScale.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool CheckIfLeftWallJump()
    {
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position, new Vector2(-1, 0), Mathf.Infinity);
        Debug.DrawRay(transform.position, new Vector2(-1 * transform.lossyScale.x * 0.6f, 0), Color.red, 0.25f);
        if (hit[1].collider.gameObject.layer == 6 && hit[1].distance < transform.localScale.x * 0.6f)// && hit[1].distance > 0.6f * transform.localScale.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool CheckIfRightWallJump()
    {
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position, new Vector2(-1, 0), Mathf.Infinity);
        Debug.DrawRay(transform.position, new Vector2(transform.lossyScale.x * 0.6f, 0), Color.red, 0.25f);
        if (hit[1].collider.gameObject.layer == 6 && hit[1].distance < transform.localScale.x * 0.6f)// && hit[1].distance > 0.6f * transform.localScale.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
