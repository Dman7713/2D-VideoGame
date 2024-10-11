using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaveGenerator : MonoBehaviour
{
    public GameObject floorPrefab; // Reference to the floor prefab
    public Transform player; // Reference to the player

    public int width = 50;
    public int height = 50;
    public float fillProbability = 0.45f;
    public int iterations = 5;
    public float renderDistance = 10f; // Distance around the player to render
    public int chunkSize = 10; // Size of each chunk

    private int[,] map;
    private Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        GenerateCave();
        StartCoroutine(UpdateCave());
    }

    void GenerateCave()
    {
        map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = Random.value < fillProbability ? 1 : 0; // 1 for wall, 0 for empty
            }
        }

        for (int i = 0; i < iterations; i++)
        {
            map = SmoothMap(map);
        }

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
                    newMap[x, y] = 1; // wall
                else if (surroundingWalls < 4)
                    newMap[x, y] = 0; // empty
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

    IEnumerator UpdateCave()
    {
        while (true)
        {
            int playerX = Mathf.FloorToInt(player.position.x / chunkSize);
            int playerY = Mathf.FloorToInt(player.position.y / chunkSize);

            // Generate or update chunks within render distance
            for (int x = playerX - Mathf.FloorToInt(renderDistance / chunkSize); x <= playerX + Mathf.FloorToInt(renderDistance / chunkSize); x++)
            {
                for (int y = playerY - Mathf.FloorToInt(renderDistance / chunkSize); y <= playerY + Mathf.FloorToInt(renderDistance / chunkSize); y++)
                {
                    Vector2Int chunkPosition = new Vector2Int(x, y);

                    if (!chunks.ContainsKey(chunkPosition))
                    {
                        // Create and store a new chunk
                        GameObject chunk = new GameObject($"Chunk_{x}_{y}");
                        chunks[chunkPosition] = chunk;
                        GenerateChunk(chunkPosition, chunk);
                    }
                }
            }

            // Clean up chunks that are out of range
            List<Vector2Int> keysToRemove = new List<Vector2Int>();
            foreach (var kvp in chunks)
            {
                if (Mathf.Abs(kvp.Key.x - playerX) > Mathf.FloorToInt(renderDistance / chunkSize) ||
                    Mathf.Abs(kvp.Key.y - playerY) > Mathf.FloorToInt(renderDistance / chunkSize))
                {
                    keysToRemove.Add(kvp.Key);
                    Destroy(kvp.Value); // Destroy the chunk GameObject
                }
            }

            foreach (var key in keysToRemove)
            {
                chunks.Remove(key);
            }

            yield return new WaitForSeconds(1f); // Adjust wait time as needed
        }
    }

    void GenerateChunk(Vector2Int chunkPosition, GameObject chunk)
    {
        for (int x = chunkPosition.x * chunkSize; x < (chunkPosition.x + 1) * chunkSize; x++)
        {
            for (int y = chunkPosition.y * chunkSize; y < (chunkPosition.y + 1) * chunkSize; y++)
            {
                if (x >= 0 && y >= 0 && x < width && y < height && map[x, y] == 0)
                {
                    Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity, chunk.transform);
                }
            }
        }
    }
}
