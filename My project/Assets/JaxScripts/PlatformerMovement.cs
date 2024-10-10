using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerMovement : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 5f;
    [SerializeField]
    float jumpSpeed = 2f;
    bool grounded = false;
    [SerializeField]
    float cyoteTime = 2;
    [SerializeField]
    float timer = 0;
    bool canJump = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
        velocity.x = moveX * moveSpeed;
        GetComponent<Rigidbody2D>().velocity = velocity;
        //need to find a way to know if we are on the ground
        timer += Time.deltaTime;
        if(timer > cyoteTime && !grounded)
        {
        canJump = false;

        }
        if (Input.GetButtonDown("Jump") && canJump)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 100 * jumpSpeed));
            grounded = false;
            canJump = false;
        }
         
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            grounded = true;
            canJump = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            timer = 0;
            grounded = false;
        }
    }

}
