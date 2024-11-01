using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    [SerializeField] private Tilemap caveTilemap;

    [Header("Biome and Border Tiles")]
    [SerializeField] private TileBase[] biomeFloorTiles; // Tiles for different biomes
    [SerializeField] private TileBase borderTile; // Tile for the border

    [Header("Map Dimensions")]
    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;
    [SerializeField] private int borderWidth = 2; // Adjustable border width

    [Header("Biome Settings")]
    [SerializeField] private int biomeCount = 5;
    [SerializeField] private float biomeRadius = 30f;
    [SerializeField] private float noiseScale = 0.1f;
    [SerializeField] private float noiseThreshold = 0.5f;
    [SerializeField] private float stoneNoiseScale = 0.1f;
    [SerializeField] private float stoneNoiseThreshold = 0.5f;

    [Header("Border Noise Settings")]
    [SerializeField] private float borderNoiseScale = 0.1f; // Scale of noise variation on the border
    [SerializeField] private float borderNoiseIntensity = 0.5f; // Intensity of the border noise effect

    [Header("Player Settings")]
    [SerializeField] private Transform player;

    private Dictionary<int, Vector2Int> biomeCenters = new Dictionary<int, Vector2Int>();

    private void Start()
    {
        GenerateCave();
        SpawnPlayerAtCenter();
    }

    void GenerateCave()
    {
        caveTilemap.ClearAllTiles();
        biomeCenters.Clear();

        CreateBiomes();
        RemoveStoneInBiomes();
        FillGapsWithStone();
        GenerateBorder();

        Debug.Log("Cave with biomes and irregular border generated.");
    }

    void SpawnPlayerAtCenter()
    {
        if (player != null)
        {
            Vector3 centerPosition = new Vector3(width / 2f, height / 2f, 0);
            player.position = centerPosition;
        }
        else
        {
            Debug.LogError("Player not assigned.");
        }
    }

    void CreateBiomes()
    {
        Vector2Int[] biomePositions = new Vector2Int[]
        {
            new Vector2Int(0, height - 1),
            new Vector2Int(width - 1, height - 1),
            new Vector2Int(0, 0),
            new Vector2Int(width - 1, 0)
        };

        for (int i = 0; i < biomePositions.Length; i++)
        {
            Vector2Int biomePosition = biomePositions[i];
            if (IsBiomePositionValid(biomePosition, biomeRadius))
            {
                biomeCenters[i] = biomePosition;
                CreateIrregularBiome(biomePosition, i);
            }
            else
            {
                Debug.LogWarning($"Biome {i} at position {biomePosition} overlaps with existing biomes.");
            }
        }
    }

    bool IsBiomePositionValid(Vector2Int position, float radius)
    {
        foreach (var biomeCenter in biomeCenters.Values)
        {
            if (Vector2Int.Distance(position, biomeCenter) < radius)
            {
                return false;
            }
        }
        return true;
    }

    void CreateIrregularBiome(Vector2Int center, int biomeIndex)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float distanceToCenter = Vector2Int.Distance(center, new Vector2Int(x, y));
                if (distanceToCenter <= biomeRadius &&
                    Mathf.PerlinNoise((x + center.x) * noiseScale, (y + center.y) * noiseScale) > noiseThreshold)
                {
                    caveTilemap.SetTile(new Vector3Int(x, y, 0), biomeFloorTiles[biomeIndex]);
                }
            }
        }
    }

    void RemoveStoneInBiomes()
    {
        foreach (var biomeCenter in biomeCenters)
        {
            Vector2Int center = biomeCenter.Value;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (Vector2Int.Distance(center, new Vector2Int(x, y)) < biomeRadius)
                    {
                        if (caveTilemap.GetTile(new Vector3Int(x, y, 0)) == biomeFloorTiles[biomeCount - 1])
                        {
                            caveTilemap.SetTile(new Vector3Int(x, y, 0), null);
                        }
                    }
                }
            }
        }
    }

    void FillGapsWithStone()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                if (caveTilemap.GetTile(tilePosition) == null && !IsInAnyBiomeRadius(x, y))
                {
                    if (Mathf.PerlinNoise(x * stoneNoiseScale, y * stoneNoiseScale) < stoneNoiseThreshold)
                    {
                        caveTilemap.SetTile(tilePosition, biomeFloorTiles[biomeCount - 1]);
                    }
                }
            }
        }
    }

    bool IsInAnyBiomeRadius(int x, int y)
    {
        Vector2Int position = new Vector2Int(x, y);
        foreach (var biomeCenter in biomeCenters.Values)
        {
            if (Vector2Int.Distance(biomeCenter, position) < biomeRadius)
            {
                return true;
            }
        }
        return false;
    }

    void GenerateBorder()
    {
        for (int x = -borderWidth; x < width + borderWidth; x++)
        {
            for (int y = -borderWidth; y < height + borderWidth; y++)
            {
                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    float noiseValue = Mathf.PerlinNoise(x * borderNoiseScale, y * borderNoiseScale);
                    if (noiseValue > borderNoiseIntensity) // Only apply tile if noise exceeds threshold
                    {
                        caveTilemap.SetTile(new Vector3Int(x, y, 0), borderTile);
                    }
                }
            }
        }
    }
}
