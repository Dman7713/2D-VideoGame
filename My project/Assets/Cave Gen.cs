using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveGenerator : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap
    public TileBase floorTile; // Reference to the Tile (Rule Tile or any TileBase)
    public int width = 50; // Width of the tilemap
    public int height = 50; // Height of the tilemap
    public float fillProbability = 0.45f; // Initial fill probability
    public int smoothingIterations = 5; // Number of smoothing iterations

    private int[,] map;

    private void Start()
    {
        GenerateCave();
    }

    void GenerateCave()
    {
        map = new int[width, height];

        // Randomly fill the map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = Random.value < fillProbability ? 1 : 0; // 1 for wall, 0 for empty
            }
        }

        // Smooth the map
        for (int i = 0; i < smoothingIterations; i++)
        {
            map = SmoothMap(map);
        }

        // Populate the Tilemap with the final cave layout
        PopulateTilemap();
        Debug.Log("Cave generated.");
    }

    int[,] SmoothMap(int[,] oldMap)
    {
        int[,] newMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int surroundingWalls = GetSurroundingWallCount(oldMap, x, y);
                if (surroundingWalls > 4)
                    newMap[x, y] = 1; // Wall
                else if (surroundingWalls < 4)
                    newMap[x, y] = 0; // Empty
                else
                    newMap[x, y] = oldMap[x, y]; // Preserve state
            }
        }

        return newMap;
    }

    int GetSurroundingWallCount(int[,] map, int x, int y)
    {
        int wallCount = 0;

        for (int nx = x - 1; nx <= x + 1; nx++)
        {
            for (int ny = y - 1; ny <= y + 1; ny++)
            {
                if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                {
                    if (nx == x && ny == y) continue; // Skip the current tile
                    wallCount += map[nx, ny];
                }
            }
        }

        return wallCount;
    }

    void PopulateTilemap()
    {
        tilemap.ClearAllTiles(); // Clear previous tiles before populating
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0) // 0 represents empty space
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), floorTile); // Set the tile
                }
            }
        }
    }
}
