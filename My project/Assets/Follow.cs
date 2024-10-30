using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Transform player; // Reference to the player's Transform
    [SerializeField] private float followSpeed = 5f; // Speed at which the object follows the player
    [SerializeField] private Vector3 offset; // Offset position from the player

    private void Update()
    {
        if (player != null)
        {
            Follow();
        }
        else
        {
            Debug.LogWarning("Player Transform not assigned.");
        }
    }

    private void Follow()
    {
        // Calculate the target position
        Vector3 targetPosition = player.position + offset;

        // Move the object towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
