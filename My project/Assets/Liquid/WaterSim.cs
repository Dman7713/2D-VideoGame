using UnityEngine;
using UnityEngine.Tilemaps;

public class FallingTileBehavior : MonoBehaviour
{
    private Tilemap tilemap;
    private Vector3Int currentPosition;
    private float moveDownInterval = 1f; // Move down every second
    private float timer = 0f; // Timer to keep track of falling interval

    private void Start()
    {
        // Find the Tilemap component in the parent of this GameObject
        tilemap = GetComponentInParent<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("Tilemap component not found! Please ensure this GameObject is a child of a Tilemap GameObject.");
            return;
        }

        // Initialize the current position based on the GameObject's world position
        currentPosition = tilemap.WorldToCell(transform.position);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Move down every moveDownInterval seconds
        if (timer >= moveDownInterval)
        {
            MoveDown();
            timer = 0f; // Reset the timer
        }
    }

    private void MoveDown()
    {
        // Check if the tile can move down
        Vector3Int newPosition = currentPosition + Vector3Int.down;
        if (IsCellEmpty(newPosition))
        {
            // Move down in the Tilemap
            currentPosition = newPosition;
            UpdateTilePosition();
        }
        else
        {
            // If can't move down, check for left/right movement
            if (!TryMoveLeft() && !TryMoveRight())
            {
                // Stop moving if it can't go anywhere
                return;
            }
        }
    }

    private bool TryMoveLeft()
    {
        Vector3Int newPosition = currentPosition + Vector3Int.left;
        if (IsCellEmpty(newPosition))
        {
            currentPosition = newPosition;
            UpdateTilePosition();
            return true;
        }
        return false;
    }

    private bool TryMoveRight()
    {
        Vector3Int newPosition = currentPosition + Vector3Int.right;
        if (IsCellEmpty(newPosition))
        {
            currentPosition = newPosition;
            UpdateTilePosition();
            return true;
        }
        return false;
    }

    private bool IsCellEmpty(Vector3Int position)
    {
        // Check if the specified cell in the tilemap is empty
        return tilemap.GetTile(position) == null;
    }

    private void UpdateTilePosition()
    {
        // Update the tile position in the tilemap
        tilemap.SetTile(currentPosition, tilemap.GetTile(currentPosition)); // Set the tile in the new position
        tilemap.SetTile(currentPosition + Vector3Int.up, null); // Clear the old position
    }
}
