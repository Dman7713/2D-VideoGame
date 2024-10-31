using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterAI : MonoBehaviour
{
    GameObject player;
    [SerializeField]
    float chaseSpeed = 10f;
    [SerializeField]
    float chaseTriggerDistance = 5.0f;
    public Transform[] PatrolPoints;
    public float moveSpeed;
    public int patrolDestination = 0; // Initial patrol destination

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // If the player gets too close, start chasing
        Vector3 playerPosition = player.transform.position;
        Vector3 chaseDir = playerPosition - transform.position;

        if (chaseDir.magnitude < chaseTriggerDistance)
        {
            // Chase the player
            chaseDir.Normalize();
            GetComponent<Rigidbody2D>().velocity = chaseDir * chaseSpeed;
        }
        else
        {
            Patrol();
        }
    }

    // Patrol behavior when player is out of range
    void Patrol()
    {
        if (patrolDestination == 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, PatrolPoints[0].position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, PatrolPoints[0].position) < 0.2f)
            {
                transform.localScale = new Vector3(1, 1, 1);
                patrolDestination = 1;
            }
        }
        else if (patrolDestination == 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, PatrolPoints[1].position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, PatrolPoints[1].position) < 0.2f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                patrolDestination = 0;
            }
        }
    }
}
