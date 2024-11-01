using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    [SerializeField] private Tilemap caveTilemap;
    [SerializeField] private Tilemap bgTilemap; // Tilemap for background tiles
    [SerializeField] private Tilemap radiusTilemap; // New Tilemap for filling radius around biomes
    [SerializeField] private TileBase borderTile; // Tile for the border

    [Header("Biome Tiles")]
    [SerializeField] private TileBase[] biomeFloorTiles; // Different tiles for each biome
    [SerializeField] private TileBase[] biomeBackgroundTiles; // Different background tiles for each biome
    [SerializeField] private TileBase radiusTile; // Tile to fill in the radius around each biome

    [Header("Overlay Prefabs")]
    [SerializeField] private GameObject[] biomeOverlayPrefabs; // Prefabs for overlay effects in each biome

    [Header("Map Dimensions")]
    [SerializeField] private int width = 100, height = 100;

    [Header("Biome Settings")]
    [SerializeField] private int biomeCount = 5;
    [SerializeField] private float biomeRadius = 30f;
    [SerializeField] private float noiseScale = 0.1f, noiseThreshold = 0.5f;

    [Header("Radius Settings")]
    [SerializeField] private float radiusSize = 15f; // New variable for radius size

    [Header("Stone Settings")]
    [SerializeField] private float stoneNoiseScale = 0.1f; // Scale for stone noise
    [SerializeField] private float stoneNoiseThreshold = 0.5f; // Threshold for stone placement

    [Header("Border Settings")]
    [SerializeField] private int borderWidth = 5;
    [SerializeField] private float borderNoiseScale = 0.1f;
    [SerializeField] private float borderNoiseThreshold = 0.5f;

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform player;

    private Dictionary<int, Vector2Int> biomeCenters = new Dictionary<int, Vector2Int>();
    private GameObject activeOverlay;
    private int currentBiome = -1;

    private void Start()
    {
        GenerateCave();
        SpawnPlayerAtCenter();
    }

    private void Update()
    {
        UpdateOverlayBasedOnBiome();
    }

    void GenerateCave()
    {
        caveTilemap.ClearAllTiles();
        bgTilemap.ClearAllTiles(); // Clear the background tilemap
        radiusTilemap.ClearAllTiles(); // Clear the radius tilemap
        biomeCenters.Clear();

        CreateBiomes();
        FillBiomeBackgrounds(); // Fill the background for each biome
        RemoveStoneInBiomes();
        FillGapsWithStone();
        FillBiomeRadius(); // Fill the radius around biomes
        CreateFadingBorder();
        Debug.Log("Cave with unique-shaped biomes, fading border, and radius filled generated.");
    }

    void FillBiomeRadius()
    {
        foreach (var biomeCenter in biomeCenters)
        {
            Vector2Int center = biomeCenter.Value;
            for (int x = center.x - (int)radiusSize; x <= center.x + (int)radiusSize; x++)
            {
                for (int y = center.y - (int)radiusSize; y <= center.y + (int)radiusSize; y++)
                {
                    if (Vector2Int.Distance(center, new Vector2Int(x, y)) <= radiusSize)
                    {
                        radiusTilemap.SetTile(new Vector3Int(x, y, 0), radiusTile);
                    }
                }
            }
        }
    }

    void CreateFadingBorder()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate distance to the nearest edge
                int distanceToEdge = Mathf.Min(x, y, width - 1 - x, height - 1 - y);

                if (distanceToEdge < borderWidth)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);

                    // Perlin noise creates the fading effect towards the inner tiles
                    float noiseValue = Mathf.PerlinNoise(x * borderNoiseScale, y * borderNoiseScale);
                    float fadeFactor = Mathf.InverseLerp(0, borderWidth, distanceToEdge);  // Smooth fading

                    // Set tile based on noise and distance to the edge
                    if (noiseValue > borderNoiseThreshold * fadeFactor)
                    {
                        caveTilemap.SetTile(tilePosition, borderTile);
                    }
                }
            }
        }
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

        // Fill edges of the biome to ensure it merges into the border
        FillBiomeEdges(center, biomeIndex);
    }

    void FillBiomeEdges(Vector2Int center, int biomeIndex)
    {
        // Fill tiles around the biome center to ensure it merges into the border
        for (int x = center.x - 1; x <= center.x + 1; x++)
        {
            for (int y = center.y - 1; y <= center.y + 1; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    caveTilemap.SetTile(new Vector3Int(x, y, 0), biomeFloorTiles[biomeIndex]);
                }
            }
        }
    }

    void FillBiomeBackgrounds()
    {
        foreach (var biomeCenter in biomeCenters)
        {
            Vector2Int center = biomeCenter.Value;
            for (int x = center.x - (int)biomeRadius; x <= center.x + (int)biomeRadius; x++)
            {
                for (int y = center.y - (int)biomeRadius; y <= center.y + (int)biomeRadius; y++)
                {
                    if (Vector2Int.Distance(center, new Vector2Int(x, y)) <= biomeRadius)
                    {
                        bgTilemap.SetTile(new Vector3Int(x, y, 0), biomeBackgroundTiles[biomeCenter.Key]);
                    }
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
                if (caveTilemap.GetTile(tilePosition) == null && !IsInAnyBiomeRadius(new Vector2Int(x, y)))
                {
                    // Place stone if the tile is empty and not near a biome
                    float noiseValue = Mathf.PerlinNoise(x * stoneNoiseScale, y * stoneNoiseScale);
                    if (noiseValue > stoneNoiseThreshold)
                    {
                        caveTilemap.SetTile(tilePosition, biomeFloorTiles[biomeCount - 1]); // Assume the last tile is the stone tile
                    }
                }
            }
        }
    }

    bool IsInAnyBiomeRadius(Vector2Int position)
    {
        foreach (var biomeCenter in biomeCenters.Values)
        {
            if (Vector2Int.Distance(position, biomeCenter) < biomeRadius)
            {
                return true;
            }
        }
        return false;
    }

    void UpdateOverlayBasedOnBiome()
    {
        // Update biome overlay based on player's position and current biome
        Vector2Int playerPosition = new Vector2Int(Mathf.FloorToInt(player.position.x), Mathf.FloorToInt(player.position.y));
        int biomeIndex = GetCurrentBiome(playerPosition);

        if (biomeIndex != currentBiome)
        {
            currentBiome = biomeIndex;
            if (activeOverlay != null)
            {
                Destroy(activeOverlay);
            }
            if (currentBiome >= 0)
            {
                activeOverlay = Instantiate(biomeOverlayPrefabs[currentBiome], transform);
            }
        }
    }

    int GetCurrentBiome(Vector2Int position)
    {
        for (int i = 0; i < biomeCenters.Count; i++)
        {
            if (Vector2Int.Distance(position, biomeCenters[i]) < biomeRadius)
            {
                return i;
            }
        }
        return -1; // Not in any biome
    }
}
