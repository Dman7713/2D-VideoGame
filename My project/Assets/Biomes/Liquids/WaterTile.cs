using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterTile : MonoBehaviour
{
    public float moveSpeed = 1f; // Speed at which the water flows
    public int flowDistance = 1; // How far the water can spread
    public GameObject waterTilePrefab; // Prefab for additional water tiles
    private bool isSource = false; // Indicates if this tile is a source tile
    private Rigidbody2D rb; // Rigidbody2D component
    private Tilemap tilemap; // Reference to the Tilemap
    private Vector3Int position; // Position of the water tile

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tilemap = FindObjectOfType<Tilemap>(); // Get the Tilemap in the scene
    }

    void Start()
    {
        position = tilemap.WorldToCell(transform.position);

        if (isSource)
        {
            rb.gravityScale = 0; // Source tile should not fall
            SpreadWater(); // Start spreading water
        }
        else
        {
            rb.gravityScale = 1; // Non-source tiles should fall
            rb.velocity = Vector2.down * moveSpeed; // Set initial downward velocity
        }
    }

    void Update()
    {
        if (!isSource)
        {
            // Check for tile below
            if (CheckTile(position + Vector3Int.down) == null)
            {
                rb.velocity = Vector2.down * moveSpeed; // Keep falling
            }
            else
            {
                rb.velocity = Vector2.zero; // Stop falling
                SpreadWater(); // Start spreading
            }
        }
    }

    private void SpreadWater()
    {
        for (int i = 1; i <= flowDistance; i++)
        {
            CheckAndSpread(Vector3Int.left, i);
            CheckAndSpread(Vector3Int.right, i);
            CheckAndSpread(Vector3Int.down, i);
        }
    }

    private void CheckAndSpread(Vector3Int direction, int distance)
    {
        Vector3Int targetPosition = position + (direction * distance);
        GameObject targetTile = CheckTile(targetPosition);

        if (targetTile == null) // If the target position is empty
        {
            SpawnWaterTile(targetPosition); // Spawn a new water tile
        }
    }

    private GameObject CheckTile(Vector3Int position)
    {
        // Implement your tile checking logic here
        TileBase tile = tilemap.GetTile(position);
        return tile != null ? tilemap.GetInstantiatedObject(position) : null;
    }

    private void SpawnWaterTile(Vector3Int position)
    {
        GameObject newTile = Instantiate(waterTilePrefab, tilemap.GetCellCenterWorld(position), Quaternion.identity);
        newTile.GetComponent<WaterTile>().SetAsFlowingTile();
    }

    public void SetAsSource()
    {
        isSource = true; // Mark this tile as a source tile
        rb.gravityScale = 0; // Prevent it from falling
        SpreadWater(); // Start spreading water
    }

    public void SetAsFlowingTile()
    {
        isSource = false; // Mark this tile as a flowing tile
        rb.gravityScale = 1; // Allow it to fall
        rb.velocity = Vector2.down * moveSpeed; // Set initial downward velocity
    }
}
