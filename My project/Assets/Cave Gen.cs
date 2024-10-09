using UnityEngine;

public class CaveGenerator : MonoBehaviour
{
    public GameObject wallPrefab; // Reference to the wall prefab
    public GameObject floorPrefab; // Reference to the floor prefab (optional)

    public int width = 50;
    public int height = 50;
    public float fillProbability = 0.45f;
    public int iterations = 5;
    private int[,] map;

    void Start()
    {
        GenerateCave();
    }

    void GenerateCave()
    {
        // Initialize the map with random values
        map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = Random.value < fillProbability ? 1 : 0; // 1 for wall, 0 for empty
            }
        }

        // Apply Cellular Automata
        for (int i = 0; i < iterations; i++)
        {
            map = SmoothMap(map);
        }

        // Create the cave in the game world
        CreateCave();
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
                    newMap[x, y] = 1;
                else if (surroundingWalls < 4)
                    newMap[x, y] = 0;
                else
                    newMap[x, y] = oldMap[x, y];
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

    void CreateCave()
    {
        // Instantiate tiles based on the map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    // Instantiate wall prefab
                    Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                else
                {
                    // Optional: Instantiate floor prefab if desired
                    Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
            }
        }
    }
}
