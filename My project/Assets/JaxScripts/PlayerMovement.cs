using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float horizontalInput;
    Rigidbody2D rb;

    [SerializeField]
    float moveSpeed = 5f;
    [SerializeField]
    bool isFacingRight = true;
    [SerializeField]
    float jumpPower = 4f;
    bool isJumping = false;
    [SerializeField]
    float coyoteTime = 1;
    bool canJump = true;
    [SerializeField]
    float timer = 0;

    [Header("WallMovement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding;
    bool grounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if (timer > coyoteTime && !grounded)
        {
            canJump = false;
        }

        FlipSprite();

        if (Input.GetButtonDown("Jump") && !isJumping && grounded && canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isJumping = true;
            grounded = false;
            canJump = false;
        }

        //set up timer
        timer += Time.deltaTime;
        ProcessWallSlide();


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
        isJumping = false;
        if (collision.gameObject.layer == 6)
        {
            grounded = true;
            canJump = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            timer = 0;
            grounded = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        timer = 0;
    }
}
