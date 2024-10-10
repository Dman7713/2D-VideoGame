using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroy : MonoBehaviour
{
    public GameObject destructionEffectPrefab; // Prefab for the destruction effect
    public float destroyDelay = 0.5f; // Time to wait before destroying the destruction effect

    [SerializeField]
    GameObject bloodPrefab; // Blood prefab to be placed when the bullet is destroyed
    [SerializeField]
    float bloodDuration = 1f; // Duration for how long the blood prefab should stay

    public AudioClip collisionSound; // Sound to play on collision
    private AudioSource audioSource; // Reference to the AudioSource component

    private void Start()
    {
        // Add an AudioSource component to the bullet if it doesn't have one
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object the bullet collides with has the tag "Wall", "Enemy", "Player", or "Bullet"
        if (collision.gameObject.CompareTag("Enemy") ||
            collision.gameObject.CompareTag("Wall") ||
            collision.gameObject.CompareTag("Player") ||
            collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Collision detected with: " + collision.gameObject.name); // Debugging line

            // Play the collision sound
            PlayCollisionSound();

            // Instantiate the destruction effect prefab at the bullet's position and rotation
            if (destructionEffectPrefab != null)
            {
                GameObject effect = Instantiate(destructionEffectPrefab, transform.position, transform.rotation);
                Destroy(effect, destroyDelay); // Destroy the effect after the specified delay
            }

            // Instantiate the blood prefab at the bullet's position if colliding with an enemy
            if (collision.gameObject.CompareTag("Enemy") && bloodPrefab != null)
            {
                GameObject bloodInstance = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
                Destroy(bloodInstance, bloodDuration); // Destroy the blood prefab after the specified duration
            }

            // Immediately destroy the bullet
            Destroy(gameObject);
        }
    }

    private void PlayCollisionSound()
    {
        if (audioSource != null && collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound); // Play the collision sound
        }
        else
        {
            Debug.LogWarning("Audio source or collision sound is not set."); // Warning if sound is not playing
        }
    }
}
