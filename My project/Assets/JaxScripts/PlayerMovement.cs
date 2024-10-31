using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    float horizontalInput;
        Rigidbody2D rb;


    [SerializeField]
    float moveSpeed = 5f;
    [SerializeField]
    bool isFaceingRight = true;
    [SerializeField]
    float jumpPower = 4f;
    bool isJumping = false;
    [SerializeField]
    float coyoteTime = 1;
    bool canJump = true;
    [SerializeField]
    float timer = 0;


    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize;
    public LayerMask wallLayer;

    [Header("WallMovement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding;
    bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if(timer > coyoteTime && !grounded)
        {
            canJump = false;
        }

        FlipSprite();

        if(Input.GetButtonDown("Jump") && !isJumping && grounded && canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isJumping = true;
            grounded = false;
            canJump = false;
        }
        //set up timer
        timer += Time.deltaTime;

        
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }
    void FlipSprite() 
    { 
        if(isFaceingRight && horizontalInput < 0f || !isFaceingRight && horizontalInput > 0f)
        {
            isFaceingRight = !isFaceingRight;
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
    private void ProcessWallSlide()
    {
        if(!grounded & WallCheck() & horizontalInput != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed)); // caps fall rate
        }else
        {
            isWallSliding = false;
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
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

}
