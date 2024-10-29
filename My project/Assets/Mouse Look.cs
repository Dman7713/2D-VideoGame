using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Front Reference")]
    [SerializeField] private Transform frontReference; // Assign the empty GameObject here

    private void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;
    }

    private void Update()
    {
        LookAtMouse();
    }

    private void LookAtMouse()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f)); // Use the appropriate distance for 2D

        // Calculate the direction from the object to the mouse position
        Vector3 direction = mousePosition - transform.position;
        direction.z = 0; // Ensure we only deal with 2D (ignore Z-axis)

        // If there's a direction, rotate the object to look at the mouse
        if (direction != Vector3.zero)
        {
            // Calculate the angle to rotate towards the mouse position
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Convert from radians to degrees

            // Set the rotation of the object
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 500f); // Adjust the speed as necessary
        }

        // Update the position of the front reference to match the front of the GameObject
        if (frontReference != null)
        {
            frontReference.position = transform.position + (direction.normalized * 0.5f); // Adjust distance as needed
        }
    }
}
