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

    void CreateBiomesWithIrregularShapes()
    {
        if (biomeCount < 4)
        {
            Debug.LogWarning("At least 4 biomes are required to cover all corners.");
            return;
        }

        List<Vector2Int> biomeCenters = new List<Vector2Int>
        {
            new Vector2Int(biomeMaxDistance / 2, height - biomeMaxDistance / 2), // Top-left corner
            new Vector2Int(width - biomeMaxDistance / 2, height - biomeMaxDistance / 2), // Top-right corner
            new Vector2Int(biomeMaxDistance / 2, biomeMaxDistance / 2), // Bottom-left corner
            new Vector2Int(width - biomeMaxDistance / 2, biomeMaxDistance / 2) // Bottom-right corner
        };

        for (int i = 0; i < 4; i++)
        {
            CreateIrregularBiome(biomeCenters[i], i);
        }

        if (biomeCount >= 5)
        {
            Vector2Int centerBiomePosition = new Vector2Int(width / 2, height / 2);
            CreateIrregularBiome(centerBiomePosition, 4);
        }

        for (int i = 5; i < biomeCount; i++)
        {
            Vector2Int randomBiomePosition;
            do
            {
                randomBiomePosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            } while (IsNearCorner(randomBiomePosition));

            CreateIrregularBiome(randomBiomePosition, i);
        }
    }

    bool IsNearCorner(Vector2Int position)
    {
        int cornerDistance = 10; // distance threshold to consider as a corner
        return (position.x < cornerDistance || position.x > width - cornerDistance) &&
               (position.y < cornerDistance || position.y > height - cornerDistance);
    }

    void CreateIrregularBiome(Vector2Int center, int biomeIndex)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate distance from the center
                float distance = Vector2Int.Distance(center, new Vector2Int(x, y));
                // Create an irregular shape using Perlin noise
                float noiseValue = Mathf.PerlinNoise((x + center.x) * noiseScale, (y + center.y) * noiseScale);

                // Use both distance and noise to create a more organic shape
                if (distance <= biomeMaxDistance && noiseValue > noiseThreshold)
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
                    float distanceFromCenter = Vector2Int.Distance(new Vector2Int(x, y), new Vector2Int(width / 2, height / 2));
                    float noiseValue = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);

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
        // Add logic for falling liquid tile behavior if needed
    }

    void CreateLiquidTile(Vector2Int position, TileBase liquidTile, bool isSource)
    {
        GameObject liquidGameObject = new GameObject("Liquid");
        liquidGameObject.transform.position = new Vector3(position.x, position.y, 0);

        // Add additional liquid behavior as needed (e.g., Rigidbody2D for physics, flow mechanics)
    }
}
