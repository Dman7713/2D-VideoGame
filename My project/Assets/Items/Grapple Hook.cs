using System.Collections;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public float swingForce = 10f; // Force applied for swinging
    public Transform player; // Reference to the player's transform
    private Vector2 grapplePoint; // Point where the player is grappled
    private bool isGrappling = false; // Track if the player is grappling
    private Rigidbody2D playerRb; // Reference to the player's Rigidbody2D

    void Start()
    {
        playerRb = player.GetComponent<Rigidbody2D>(); // Get the player's Rigidbody2D
    }

    void Update()
    {
        // Check for right mouse button input
        if (Input.GetMouseButtonDown(1) && !isGrappling) // Right mouse button (button 1)
        {
            TryGrapple();
        }

        // Swinging logic
        if (isGrappling)
        {
            SwingPlayer();

            if (Input.GetMouseButtonUp(1)) // Release the right mouse button
            {
                ReleaseGrapple();
            }
        }
    }

    void TryGrapple()
    {
        // Cast a ray from the camera to the mouse position
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // Check if we hit an object on the grapple layer
        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Grappleable"))
        {
            grapplePoint = hit.point; // Set the grapple point
            isGrappling = true; // Set grappling to true
            playerRb.velocity = Vector2.zero; // Stop the player's movement abruptly
        }
    }

    void SwingPlayer()
    {
        // Calculate direction from the player to the grapple point
        Vector2 direction = (grapplePoint - (Vector2)player.position).normalized;
        Vector2 force = new Vector2(-direction.y, direction.x) * swingForce; // Perpendicular direction for swinging

        playerRb.AddForce(force * Time.deltaTime, ForceMode2D.Force); // Apply the swing force
    }

    void ReleaseGrapple()
    {
        isGrappling = false; // Set grappling to false
    }
}
