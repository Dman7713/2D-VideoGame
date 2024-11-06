using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float horizontalInput;
    Rigidbody2D rb;
    Animator animator;

    [SerializeField]
    float moveSpeed = 5f;
    [SerializeField]
    bool isFacingRight = true;
    [SerializeField]
    float jumpPower = 4f;
    bool isJumping = false;
    [SerializeField]
    float coyoteTime = 0.2f;
    bool canJump = true;
    float timer = 0;

    [Header("Jump Settings")]
    [SerializeField]
    private int maxJumpCount = 2; // Maximum jumps allowed before cooldown
    private int currentJumpCount = 0; // Current jump count

    [Header("Jump Cooldown")]
    [SerializeField]
    private float jumpCooldown = 1f; // Cooldown duration in seconds
    private float jumpCooldownTimer = 0f; // Timer to track cooldown

    [Header("Wall Movement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding;
    bool grounded;

    // Dust Particle System
    [SerializeField]
    private ParticleSystem dust; // Reference to the dust prefab

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // Update cooldown timer
        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        if (grounded)
        {
            canJump = true;
            timer = 0;
            currentJumpCount = 0; // Reset jump count when grounded
        }
        else
        {
            timer += Time.deltaTime;
        }

        FlipSprite();

        // Check if jump is allowed by cooldown and jump count
        if (Input.GetButtonDown("Jump") && (grounded || timer < coyoteTime) && currentJumpCount < maxJumpCount && jumpCooldownTimer <= 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isJumping = true;
            grounded = false;
            currentJumpCount++; // Increment jump count
            jumpCooldownTimer = jumpCooldown; // Reset cooldown timer
            CreateDust(); // Create dust when jumping
        }

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    void FlipSprite()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x < transform.position.x && isFacingRight)
        {
            isFacingRight = false;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
        else if (mousePosition.x > transform.position.x && !isFacingRight)
        {
            isFacingRight = true;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            grounded = true;
            isJumping = false;
            CreateDust(); // Create dust when landing
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            grounded = false;
        }
    }

    void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsJumping", !grounded);
    }

    void CreateDust()
    {
        if (dust != null)
        {
            dust.Play();
        }
    }
}
