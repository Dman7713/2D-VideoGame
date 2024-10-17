using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    [SerializeField] private Tilemap caveTilemap;

    [Header("Biome Tiles")]
    [SerializeField] private TileBase[] biomeFloorTiles;

    [Header("Map Dimensions")]
    [SerializeField] private int width = 100, height = 100;

    [Header("Biome Settings")]
    [SerializeField] private int biomeCount = 5, biomeMaxDistance = 30;

    [Header("Noise Settings")]
    [SerializeField] private float noiseScale = 0.1f, noiseThreshold = 0.5f;

    [Header("Ore Settings")]
    [SerializeField] private TileBase[] oreTiles;
    [SerializeField] private float oreSpawnChance = 5f;
    [SerializeField] private int minOreClusterSize = 3, maxOreClusterSize = 8;

    private void Start() => GenerateCave();

    void GenerateCave()
    {
        caveTilemap.ClearAllTiles();
        CreateBiomes();
        FillEmptySpaceWithBiome();
        GenerateOres();
        Debug.Log("Cave with unique-shaped biomes and ores generated.");
    }

    void CreateBiomes()
    {
        if (biomeCount < 4) { Debug.LogWarning("At least 4 biomes are required to cover all corners."); return; }

        Vector2Int[] biomeCenters = {
            new Vector2Int(biomeMaxDistance / 2, height - biomeMaxDistance / 2),
            new Vector2Int(width - biomeMaxDistance / 2, height - biomeMaxDistance / 2),
            new Vector2Int(biomeMaxDistance / 2, biomeMaxDistance / 2),
            new Vector2Int(width - biomeMaxDistance / 2, biomeMaxDistance / 2)
        };

        for (int i = 0; i < 4; i++) CreateIrregularBiome(biomeCenters[i], i);

        if (biomeCount >= 5) CreateIrregularBiome(new Vector2Int(width / 2, height / 2), 4);

        for (int i = 5; i < biomeCount; i++)
        {
            Vector2Int randomBiomePosition;
            do { randomBiomePosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height)); }
            while (IsNearCorner(randomBiomePosition));

            CreateIrregularBiome(randomBiomePosition, i);
        }
    }

    bool IsNearCorner(Vector2Int position) => (position.x < 10 || position.x > width - 10) && (position.y < 10 || position.y > height - 10);

    void CreateIrregularBiome(Vector2Int center, int biomeIndex)
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (Vector2Int.Distance(center, new Vector2Int(x, y)) <= biomeMaxDistance &&
                    Mathf.PerlinNoise((x + center.x) * noiseScale, (y + center.y) * noiseScale) > noiseThreshold)
                    caveTilemap.SetTile(new Vector3Int(x, y, 0), biomeFloorTiles[biomeIndex]);
    }

    void FillEmptySpaceWithBiome()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (caveTilemap.GetTile(new Vector3Int(x, y, 0)) == null &&
                    Vector2Int.Distance(new Vector2Int(x, y), new Vector2Int(width / 2, height / 2)) > biomeMaxDistance &&
                    Mathf.PerlinNoise(x * noiseScale, y * noiseScale) > noiseThreshold)
                    caveTilemap.SetTile(new Vector3Int(x, y, 0), biomeFloorTiles[biomeCount - 1]);
    }

    void GenerateOres()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (caveTilemap.GetTile(new Vector3Int(x, y, 0)) != null &&
                    Random.Range(0f, 100f) < oreSpawnChance)
                {
                    PlaceOreCluster(new Vector2Int(x, y), Random.Range(minOreClusterSize, maxOreClusterSize));
                }
    }

    void PlaceOreCluster(Vector2Int center, int size)
    {
        TileBase oreTile = oreTiles[Random.Range(0, oreTiles.Length)];
        for (int i = 0; i < size; i++)
        {
            Vector2Int orePosition = new Vector2Int(center.x + Random.Range(-2, 3), center.y + Random.Range(-2, 3));
            if (orePosition.x >= 0 && orePosition.x < width && orePosition.y >= 0 && orePosition.y < height &&
                caveTilemap.GetTile(new Vector3Int(orePosition.x, orePosition.y, 0)) != null)
            {
                caveTilemap.SetTile(new Vector3Int(orePosition.x, orePosition.y, 0), oreTile);
            }
        }
    }
}