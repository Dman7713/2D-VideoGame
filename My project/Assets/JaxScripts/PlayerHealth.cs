using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    int health = 10;
    [SerializeField]
    float timer = 0f;
    [SerializeField]
    float tickDamage = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        //we want to take damage IF the player hits the enemy capsule
        //bool key = true;
        if (collision.gameObject.tag == "Enemy")
        {
            //health = health - 1;
            health -= 1;
            //health--;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && timer > tickDamage)
        {
            health -= 1;
            timer = 0f;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
