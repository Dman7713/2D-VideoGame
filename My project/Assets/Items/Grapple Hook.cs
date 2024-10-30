using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [SerializeField] private float grappleLength; // Maximum length for the grapple
    [SerializeField] private LayerMask grappleLayer; // LayerMask to identify valid grapple surfaces
    [SerializeField] private LineRenderer rope; // LineRenderer for the grapple rope

    private Vector3 grapplePoint; // Point where the grapple attaches
    private DistanceJoint2D joint; // DistanceJoint2D for grappling

    // Start is called before the first frame update
    void Start()
    {
        joint = GetComponent<DistanceJoint2D>(); // Get the DistanceJoint2D component
        joint.enabled = false; // Initially disable the joint
        rope.enabled = false; // Initially disable the rope
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Get the world point from the mouse position
            Vector3 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            origin.z = 0; // Reset Z to zero for 2D raycast

            // Cast a ray from the player's position toward the mouse position
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (origin - transform.position).normalized, grappleLength, grappleLayer);

            if (hit.collider != null)
            {
                grapplePoint = hit.point; // Set the grapple point to the hit point
                joint.connectedAnchor = grapplePoint; // Set the joint's connected anchor
                joint.enabled = true; // Enable the joint
                joint.distance = grappleLength; // Set the distance of the joint

                // Update the LineRenderer positions
                rope.SetPosition(0, grapplePoint); // Start of the rope at the grapple point
                rope.SetPosition(1, transform.position); // End of the rope at the player
                rope.enabled = true; // Enable the rope
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopGrapple(); // Call method to stop grappling
        }

        if (rope.enabled)
        {
            // Update the LineRenderer's second position to follow the player's position
            rope.SetPosition(1, transform.position); // Update the player's position in the rope
        }
    }

    // Method to stop grappling
    private void StopGrapple()
    {
        joint.enabled = false; // Disable the joint
        rope.enabled = false; // Disable the rope
    }
}
