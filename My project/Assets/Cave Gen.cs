using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    [SerializeField] private Tilemap caveTilemap;

    [Header("Biome Tiles")]
    [SerializeField] private TileBase[] biomeFloorTiles; // Biome tiles (1-4) + Stone (5)

    [Header("Overlay Prefabs")]
    [SerializeField] private GameObject[] biomeOverlayPrefabs; // Overlay prefabs for each biome

    [Header("Map Dimensions")]
    [SerializeField] private int width = 100, height = 100;

    [Header("Biome Settings")]
    [SerializeField] private int biomeCount = 5; // Total biomes including stone
    [SerializeField] private float biomeRadius = 30f; // Radius for each biome
    [SerializeField] private float noiseScale = 0.1f, noiseThreshold = 0.5f; // For biome generation
    [SerializeField] private float stoneNoiseScale = 0.1f; // For stone biome cave noise
    [SerializeField] private float stoneNoiseThreshold = 0.5f; // For stone biome cave noise

    [Header("Ore Settings")]
    [SerializeField] private TileBase[] oreTiles;
    [SerializeField] private float oreSpawnChance = 5f;
    [SerializeField] private int minOreClusterSize = 3, maxOreClusterSize = 8;

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera; // Reference to the camera
    [SerializeField] private Transform player;  // Player reference to track position

    private Dictionary<int, Vector2Int> biomeCenters = new Dictionary<int, Vector2Int>(); // Biome centers
    private GameObject activeOverlay; // To track the current active overlay
    private int currentBiome = -1; // To track the current biome index

    private void Start()
    {
        GenerateCave();
    }

    private void Update()
    {
        UpdateOverlayBasedOnBiome();
    }

    void GenerateCave()
    {
        caveTilemap.ClearAllTiles();
        biomeCenters.Clear(); // Clear previous biome center data

        CreateBiomes();           // Create biomes in corners
        RemoveStoneInBiomes();    // Remove stone tiles in the radius of biomes 1-4
        FillGapsWithStone();      // Fill the gaps with the stone biome (Biome 5)
        GenerateOres();           // Generate ores in the cave
        Debug.Log("Cave with unique-shaped biomes and ores generated.");
    }

    void CreateBiomes()
    {
        Vector2Int[] biomePositions = new Vector2Int[]
        {
            new Vector2Int(0, height - 1),      // Top-left corner (Biome 1)
            new Vector2Int(width - 1, height - 1), // Top-right corner (Biome 2)
            new Vector2Int(0, 0),               // Bottom-left corner (Biome 3)
            new Vector2Int(width - 1, 0)        // Bottom-right corner (Biome 4)
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
                return false; // Overlap detected
            }
        }
        return true; // No overlap
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
                        if (caveTilemap.GetTile(new Vector3Int(x, y, 0)) == biomeFloorTiles[biomeCount - 1]) // Check for stone biome
                        {
                            caveTilemap.SetTile(new Vector3Int(x, y, 0), null); // Remove stone tile
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
                        caveTilemap.SetTile(tilePosition, biomeFloorTiles[biomeCount - 1]); // Set to stone biome (Biome 5)
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
                return true; // Position is within the radius of an existing biome
            }
        }
        return false; // Position is outside all biome radii
    }

    void GenerateOres()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (caveTilemap.GetTile(new Vector3Int(x, y, 0)) != null &&
                    Random.Range(0f, 100f) < oreSpawnChance)
                {
                    PlaceOreCluster(new Vector2Int(x, y), Random.Range(minOreClusterSize, maxOreClusterSize));
                }
            }
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

    int GetBiomeFromPosition(Vector3 position)
    {
        Vector2 playerPosition2D = new Vector2(position.x, position.y);
        foreach (var biome in biomeCenters)
        {
            Vector2 biomeCenter = biome.Value;
            float distanceToCenter = Vector2.Distance(playerPosition2D, biomeCenter);

            if (distanceToCenter <= biomeRadius)
            {
                return biome.Key;
            }
        }
        return -1; // Player is not in any known biome
    }

    void UpdateOverlayBasedOnBiome()
    {
        if (player == null) { Debug.LogError("Player not assigned."); return; }

        int playerBiome = GetBiomeFromPosition(player.position);
        if (playerBiome != currentBiome)
        {
            currentBiome = playerBiome;
            SwitchOverlay(playerBiome);
        }
    }

    void SwitchOverlay(int biomeIndex)
    {
        if (activeOverlay != null) Destroy(activeOverlay);

        if (biomeIndex >= 0 && biomeIndex < biomeOverlayPrefabs.Length)
        {
            activeOverlay = Instantiate(biomeOverlayPrefabs[biomeIndex], mainCamera.transform);
            activeOverlay.transform.localPosition = Vector3.zero;
        }
    }
}
