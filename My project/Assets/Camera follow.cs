using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;      // The target for the camera to follow
    public Vector2 offset;        // Offset from the target
    public float smoothTime = 0.3f; // Time for the camera to smooth to the target position
    private Vector3 velocity = Vector3.zero; // Velocity used by SmoothDamp

    void FixedUpdate() // Use FixedUpdate for more consistent results
    {
        if (target != null)
        {
            // Calculate the desired position
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);

            // Smoothly move the camera towards the desired position using SmoothDamp
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
    }
}
