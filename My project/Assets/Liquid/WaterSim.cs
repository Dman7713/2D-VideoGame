using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapFluidSimulator : MonoBehaviour
{
    public Tilemap tilemap; // Assign your tilemap in the inspector
    public RuleTile sandTile; // Assign your sand rule tile in the inspector
    public RuleTile waterTile; // Assign your water rule tile in the inspector
    public TileBase solidTile; // Assign your solid tile (ground/wall) in the inspector

    private void Update()
    {
        SimulateSand();
        SimulateWater();
    }

    private void SimulateSand()
    {
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(position) == sandTile)
                {
                    // Check if there's space below the sand tile
                    Vector3Int below = new Vector3Int(x, y - 1, 0);
                    if (tilemap.GetTile(below) == null) // Empty space below (air)
                    {
                        tilemap.SetTile(below, sandTile);
                        tilemap.SetTile(position, null);
                    }
                    else if (tilemap.GetTile(below) != solidTile) // If not a solid object, allow diagonal movement
                    {
                        Vector3Int leftBelow = new Vector3Int(x - 1, y - 1, 0);
                        Vector3Int rightBelow = new Vector3Int(x + 1, y - 1, 0);

                        if (tilemap.GetTile(leftBelow) == null)
                        {
                            tilemap.SetTile(leftBelow, sandTile);
                            tilemap.SetTile(position, null);
                        }
                        else if (tilemap.GetTile(rightBelow) == null)
                        {
                            tilemap.SetTile(rightBelow, sandTile);
                            tilemap.SetTile(position, null);
                        }
                    }
                }
            }
        }
    }

    private void SimulateWater()
    {
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(position) == waterTile)
                {
                    // Check if there's space below the water tile
                    Vector3Int below = new Vector3Int(x, y - 1, 0);
                    if (tilemap.GetTile(below) == null) // Empty space below (air)
                    {
                        tilemap.SetTile(below, waterTile);
                        tilemap.SetTile(position, null);
                    }
                    else if (tilemap.GetTile(below) != solidTile) // Spread horizontally
                    {
                        Vector3Int left = new Vector3Int(x - 1, y, 0);
                        Vector3Int right = new Vector3Int(x + 1, y, 0);

                        if (tilemap.GetTile(left) == null)
                        {
                            tilemap.SetTile(left, waterTile);
                        }
                        else if (tilemap.GetTile(right) == null)
                        {
                            tilemap.SetTile(right, waterTile);
                        }
                    }
                }
            }
        }
    }
}
