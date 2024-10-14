using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CaveGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    [SerializeField] private Tilemap caveTilemap; // Tilemap for caves
    [SerializeField] private Tilemap liquidTilemap; // Tilemap for liquids (rule tiles)

    [Header("Biome Tiles")]
    [SerializeField] private TileBase[] biomeFloorTiles;

    [Header("Liquid Tiles")]
    [SerializeField] private TileBase waterTile; // Tile for water
    [SerializeField] private TileBase lavaTile; // Tile for lava

    [Header("Map Dimensions")]
    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;

    [Header("Biome Settings")]
    [SerializeField] private int biomeCount = 5;
    [SerializeField] private int biomeMaxDistance = 30;

    [Header("Noise Settings")]
    [SerializeField] private float noiseScale = 0.1f;
    [SerializeField] private float noiseThreshold = 0.5f;

    [Header("Ore Settings")]
    [SerializeField] private TileBase[] oreTiles;
    [SerializeField] private float oreSpawnChance = 5f;
    [SerializeField] private int minOreClusterSize = 3;
    [SerializeField] private int maxOreClusterSize = 8;

    [Header("Liquid Settings")]
    [SerializeField] private int waterCount = 3; // Number of water bodies to generate
    [SerializeField] private int lavaCount = 2; // Number of lava bodies to generate
    [SerializeField] private float liquidSpawnChance = 20f; // Chance for liquids to spawn (percentage)
    [SerializeField] private float liquidMoveSpeed = 0.5f; // Speed of liquid movement

    private void Start()
    {
        GenerateCave();
        GenerateLiquids();
    }

    void GenerateCave()
    {
        caveTilemap.ClearAllTiles();
        CreateBiomesWithIrregularShapes();
        FillEmptySpaceWithBiome();
        GenerateOres();
        Debug.Log("Cave with unique-shaped biomes and ores generated.");
    }

    // Updated method for biome spawning in corners and center
    void CreateBiomesWithIrregularShapes()
    {
        // Ensure we have at least 4 biomes for the corners
        if (biomeCount < 4)
        {
            Debug.LogWarning("At least 4 biomes are required to cover all corners.");
            return;
        }

        // Define the biome centers for each corner of the map
        List<Vector2Int> biomeCenters = new List<Vector2Int>
        {
            new Vector2Int(biomeMaxDistance / 2, height - biomeMaxDistance / 2), // Top-left corner
            new Vector2Int(width - biomeMaxDistance / 2, height - biomeMaxDistance / 2), // Top-right corner
            new Vector2Int(biomeMaxDistance / 2, biomeMaxDistance / 2), // Bottom-left corner
            new Vector2Int(width - biomeMaxDistance / 2, biomeMaxDistance / 2) // Bottom-right corner
        };

        // Generate the biomes in the four corners
        for (int i = 0; i < 4; i++)
        {
            CreateIrregularBiome(biomeCenters[i], i);
        }

        // If there's a 5th biome, place it in the center of the map
        if (biomeCount == 5)
        {
            Vector2Int centerBiomePosition = new Vector2Int(width / 2, height / 2);
            CreateIrregularBiome(centerBiomePosition, 4); // 4 because it's the 5th biome (index starts at 0)
        }
    }

    bool DoesOverlap(List<Vector2Int> biomeCenters, Vector2Int newCenter, int maxDistance)
    {
        foreach (var center in biomeCenters)
        {
            if (Vector2Int.Distance(center, newCenter) < maxDistance)
            {
                return true;
            }
        }
        return false;
    }

    void CreateIrregularBiome(Vector2Int center, int biomeIndex)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float distance = Vector2Int.Distance(center, new Vector2Int(x, y));

                // Calculate noise value
                float noiseValue = Mathf.PerlinNoise((x + center.x) * noiseScale, (y + center.y) * noiseScale);

                // Check if within the biome range and the noise value
                if (distance <= biomeMaxDistance && noiseValue > noiseThreshold)
                {
                    caveTilemap.SetTile(new Vector3Int(x, y, 0), biomeFloorTiles[biomeIndex]);
                }
                // Optional: Fill more tiles near the center even if the distance is less
                if (distance <= biomeMaxDistance * 0.5f && noiseValue > noiseThreshold * 0.5f)
                {
                    caveTilemap.SetTile(new Vector3Int(x, y, 0), biomeFloorTiles[biomeIndex]);
                }
            }
        }
    }


    void FillEmptySpaceWithBiome()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (caveTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                {
                    // Check distance from the center of the map
                    float distanceFromCenter = Vector2Int.Distance(new Vector2Int(x, y), new Vector2Int(width / 2, height / 2));

                    // Modify noise generation based on distance from center
                    float noiseValue = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);

                    // Fill space only if far enough from center
                    if (distanceFromCenter > biomeMaxDistance && noiseValue > noiseThreshold)
                    {
                        caveTilemap.SetTile(new Vector3Int(x, y, 0), biomeFloorTiles[biomeCount - 1]);
                    }
                }
            }
        }
    }

    void GenerateOres()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (caveTilemap.GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    if (Random.Range(0f, 100f) < oreSpawnChance)
                    {
                        Vector2Int oreClusterCenter = new Vector2Int(x, y);
                        int clusterSize = Random.Range(minOreClusterSize, maxOreClusterSize);
                        PlaceOreCluster(oreClusterCenter, clusterSize);
                    }
                }
            }
        }
    }

    void PlaceOreCluster(Vector2Int center, int size)
    {
        TileBase oreTile = oreTiles[Random.Range(0, oreTiles.Length)];
        for (int i = 0; i < size; i++)
        {
            int offsetX = Random.Range(-2, 3);
            int offsetY = Random.Range(-2, 3);
            Vector2Int orePosition = new Vector2Int(center.x + offsetX, center.y + offsetY);

            if (orePosition.x >= 0 && orePosition.x < width && orePosition.y >= 0 && orePosition.y < height)
            {
                if (caveTilemap.GetTile(new Vector3Int(orePosition.x, orePosition.y, 0)) != null)
                {
                    caveTilemap.SetTile(new Vector3Int(orePosition.x, orePosition.y, 0), oreTile);
                }
            }
        }
    }

    void GenerateLiquids()
    {
        Debug.Log("Generating Liquids");
        GenerateLiquid(waterTile, waterCount);
        GenerateLiquid(lavaTile, lavaCount);
    }

    void GenerateLiquid(TileBase liquidTile, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2Int liquidCenter = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

            if (caveTilemap.GetTile(new Vector3Int(liquidCenter.x, liquidCenter.y, 0)) == null)
            {
                if (Random.Range(0f, 100f) < liquidSpawnChance)
                {
                    liquidTilemap.SetTile(new Vector3Int(liquidCenter.x, liquidCenter.y, 0), liquidTile);
                    CreateLiquidTile(liquidCenter, liquidTile, true);
                    SpawnFallingLiquidTiles(liquidCenter, liquidTile);
                }
            }
        }
    }

    void SpawnFallingLiquidTiles(Vector2Int liquidCenter, TileBase liquidTile)
    {
        // Implement logic to spawn surrounding liquid tiles, if necessary
    }

    void CreateLiquidTile(Vector2Int position, TileBase liquidTile, bool isSource)
    {
        GameObject liquidGameObject = new GameObject("Liquid");
        liquidGameObject.transform.position = new Vector3(position.x, position.y, 0);
        Rigidbody2D rb = liquidGameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Adjust as necessary
        rb.velocity = new Vector2(0, -liquidMoveSpeed); // Adjust liquid movement
    }
}
