using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileDestructionManager : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap
    public int defaultTileHealth = 3; // Default health for each tile
    public int damagePerClick = 1; // Amount of damage per left-click
    private Dictionary<Vector3Int, int> tileHealthDict = new Dictionary<Vector3Int, int>();
    private List<Vector3Int> tilesToRemove = new List<Vector3Int>();

    private void Update()
    {
        // Handle mouse click for damaging tiles
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0; // Ensure Z is 0 since we're in 2D
            Vector3Int tilePosition = tilemap.WorldToCell(mouseWorldPos);

            DamageTile(mouseWorldPos, damagePerClick);
        }

        // Remove tiles from the tilemap after a few frames to reduce lag
        if (tilesToRemove.Count > 0)
        {
            foreach (var tilePosition in tilesToRemove)
            {
                tilemap.SetTile(tilePosition, null);
            }
            tilesToRemove.Clear();
        }
    }

    public void DamageTile(Vector3 worldPosition, int damageAmount)
    {
        Vector3Int tilePosition = tilemap.WorldToCell(worldPosition);

        // Assign default health if the tile exists and hasn’t been initialized
        if (!tileHealthDict.ContainsKey(tilePosition) && tilemap.HasTile(tilePosition))
        {
            tileHealthDict[tilePosition] = defaultTileHealth;
        }

        if (tileHealthDict.ContainsKey(tilePosition))
        {
            tileHealthDict[tilePosition] -= damageAmount;

            if (tileHealthDict[tilePosition] <= 0)
            {
                tileHealthDict.Remove(tilePosition);
                tilesToRemove.Add(tilePosition); // Mark tile for removal
            }
        }
    }
}
