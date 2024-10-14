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

    // New parameters for liquid generation
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

    void CreateBiomesWithIrregularShapes()
    {
        List<Vector2Int> biomeCenters = new List<Vector2Int>();
        int attempts = 0;
        const int maxAttempts = 100;

        while (biomeCenters.Count < biomeCount - 1 && attempts < maxAttempts * (biomeCount - 1))
        {
            Vector2Int center = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

            if (!DoesOverlap(biomeCenters, center, biomeMaxDistance))
            {
                biomeCenters.Add(center);
                CreateIrregularBiome(center, biomeCenters.Count - 1);
            }

            attempts++;
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

                if (distance <= biomeMaxDistance)
                {
                    float noiseValue = Mathf.PerlinNoise((x + center.x) * noiseScale, (y + center.y) * noiseScale);

                    if (noiseValue > noiseThreshold * (distance / biomeMaxDistance))
                    {
                        caveTilemap.SetTile(new Vector3Int(x, y, 0), biomeFloorTiles[biomeIndex]);
                    }
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
                    float noiseValue = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);
                    if (noiseValue > noiseThreshold)
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
            // Randomly choose a position to place the liquid
            Vector2Int liquidCenter = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

            // Check if the tile can be placed (it should be empty space)
            if (caveTilemap.GetTile(new Vector3Int(liquidCenter.x, liquidCenter.y, 0)) == null)
            {
                // Decide whether to spawn the liquid based on liquidSpawnChance
                if (Random.Range(0f, 100f) < liquidSpawnChance)
                {
                    // Set the first liquid tile as the source
                    liquidTilemap.SetTile(new Vector3Int(liquidCenter.x, liquidCenter.y, 0), liquidTile);
                    // Make this tile a source by adding Rigidbody2D and BoxCollider2D
                    CreateLiquidTile(liquidCenter, liquidTile, true);

                    // Spawn additional liquid tiles around the source
                    SpawnFallingLiquidTiles(liquidCenter, liquidTile);
                }
            }
        }
    }

    void SpawnFallingLiquidTiles(Vector2Int liquidCenter, TileBase liquidTile)
    {
        // Implement logic to spawn surrounding liquid tiles, if necessary
        // This function should handle the logic of where to place additional liquid tiles
        // based on the source position. For example, you can place tiles directly below,
        // left, or right of the source.
    }

    void CreateLiquidTile(Vector2Int position, TileBase liquidTile, bool isSource)
    {
        // Here, you would implement the logic to add any necessary components (e.g., Rigidbody2D)
        // to the liquid tile for movement and physics.
        // Example:
        GameObject liquidGameObject = new GameObject("Liquid");
        liquidGameObject.transform.position = new Vector3(position.x, position.y, 0);
        Rigidbody2D rb = liquidGameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Adjust as necessary
        rb.velocity = new Vector2(0, -liquidMoveSpeed); // Adjust liquid movement

        // Add other components as needed
    }
}
