using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerMovementWithWallJump : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float accelleration = 1.0f;
    public float jumpSpeed = 1.0f;
    public bool grounded = false;
    public bool leftWall = false;
    public float wallJumpSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        var velocity = GetComponent<Rigidbody2D>().velocity;
        velocity.x += moveX * accelleration*Time.deltaTime;
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
    }
    bool CheckIfGrounded()
    {
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position, new Vector2(0, -1), Mathf.Infinity);
        Debug.Log(hit[1].collider.gameObject.name);
        Debug.DrawRay(transform.position, new Vector2(0, -1 * transform.lossyScale.y * 0.6f), Color.red, 0.25f);
        if (hit[1].collider.gameObject.layer == 8 && hit[1].distance < transform.localScale.y * 0.6f)// && hit[1].distance > 0.6f * transform.localScale.y)
        {
            return true;
        }else
        {
            return false;
        }
    }
    bool CheckIfLeftWallJump()
    {
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position, new Vector2(-1, 0), Mathf.Infinity);
        Debug.DrawRay(transform.position, new Vector2(-1 * transform.lossyScale.x * 0.6f, 0), Color.red, 0.25f);
        if (hit[1].collider.gameObject.layer == 8 && hit[1].distance < transform.localScale.x * 0.6f)// && hit[1].distance > 0.6f * transform.localScale.y)
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
        if (hit[1].collider.gameObject.layer == 8 && hit[1].distance < transform.localScale.x * 0.6f)// && hit[1].distance > 0.6f * transform.localScale.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            grounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            grounded = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            grounded = true;
        }
        ContactPoint2D[] contact = new ContactPoint2D[10];
        int points = collision.GetContacts(contact);
        //Debug.Log(contact[0].point.ToString());
        if(contact[0].point.y > transform.position.y)
        {
            if(contact[0].point.x < transform.position.x)
            {
                Debug.Log("Left");
            }
        }
        if (contact[0].point.y > transform.position.y)
        {
            if (contact[0].point.x > transform.position.x)
            {
                Debug.Log("Right");
            }
        }
    }*/

}
