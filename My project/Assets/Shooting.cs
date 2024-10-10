using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Reference to the bullet prefab
    public float bulletSpeed = 20f;  // Speed of the bullet

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check if the left mouse button is pressed
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Set z position to 0, assuming a 2D game

        // Calculate the direction from the player to the mouse position
        Vector3 shootDirection = (mousePos - transform.position).normalized;

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Set the bullet's velocity
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = shootDirection * bulletSpeed;
        }
    }
}
