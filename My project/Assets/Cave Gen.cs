using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    [SerializeField] private Tilemap caveTilemap, bgTilemap, radiusTilemap;
    [SerializeField] private TileBase borderTile;

    [Header("Outline Tilemap")]
    [SerializeField] private Tilemap outlineTilemap; // New tilemap for the outline
    [SerializeField] private TileBase outlineTile; // New tile for the outline

    [Header("Decoration Tilemap")]
    [SerializeField] private Tilemap decorationTilemap; // New tilemap for decorations
    [SerializeField] private TileBase[] decorationTiles; // Array of decoration tiles

    [Header("Biome Tiles")]
    [SerializeField] private TileBase[] biomeFloorTiles, biomeBackgroundTiles;
    [SerializeField] private TileBase radiusTile;

    [Header("Overlay Prefabs")]
    [SerializeField] private GameObject[] biomeOverlayPrefabs; // Array of overlays for biomes 0-3 (no overlay for biome 4)

    [Header("Map Settings")]
    [SerializeField] private int width = 100, height = 100;
    [SerializeField] private int biomeCount = 5, borderWidth = 5;
    [SerializeField] private float biomeRadius = 30f, radiusSize = 15f;
    [SerializeField] private float noiseScale = 0.1f, noiseThreshold = 0.5f;
    [SerializeField] private float stoneNoiseScale = 0.1f, stoneNoiseThreshold = 0.5f;
    [SerializeField] private float borderNoiseScale = 0.1f, borderNoiseThreshold = 0.5f;

    [Header("Player and Camera")]
    [SerializeField] private Transform player;

    private Dictionary<int, Vector2Int> biomeCenters = new Dictionary<int, Vector2Int>();
    private int currentBiome = -1;

    private void Start()
    {
        GenerateCave();
        DeactivateAllOverlays(); // Deactivate all overlays at the start
    }

    private void Update()
    {
        UpdateBiomeOverlay();
    }

    void GenerateCave()
    {
        ClearTilemaps();
        CreateBiomes();
        FillBackgroundTiles();
        RemoveStoneInBiomes();
        FillEmptyTilesWithStone();
        FillSquareAroundBiomes();
        CreateFadingBorder();
        CreateOutline(); // Call to create the outline after other tiles are set
        SpawnDecorationsAboveBiomes(); // Spawn decorations above cave tiles
    }

    void ClearTilemaps()
    {
        caveTilemap.ClearAllTiles();
        bgTilemap.ClearAllTiles();
        radiusTilemap.ClearAllTiles();
        outlineTilemap.ClearAllTiles(); // Clear the outline tilemap
        decorationTilemap.ClearAllTiles(); // Clear the decoration tilemap
        biomeCenters.Clear();
    }

    void CreateBiomes()
    {
        Vector2Int[] corners = { new Vector2Int(0, height - 1), new Vector2Int(width - 1, height - 1), new Vector2Int(0, 0), new Vector2Int(width - 1, 0) };
        for (int i = 0; i < corners.Length; i++)
        {
            if (IsBiomePositionValid(corners[i], biomeRadius))
            {
                biomeCenters[i] = corners[i];
                GenerateBiome(corners[i], i);
            }
        }
    }

    bool IsBiomePositionValid(Vector2Int position, float radius)
    {
        foreach (var center in biomeCenters.Values)
        {
            if (Vector2Int.Distance(position, center) < radius) return false;
        }
        return true;
    }

    void GenerateBiome(Vector2Int center, int biomeIndex)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Vector2Int.Distance(center, new Vector2Int(x, y)) <= biomeRadius &&
                    Mathf.PerlinNoise((x + center.x) * noiseScale, (y + center.y) * noiseScale) > noiseThreshold)
                {
                    caveTilemap.SetTile(new Vector3Int(x, y, 0), biomeFloorTiles[biomeIndex]);
                }
            }
        }
    }

    void FillBackgroundTiles()
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
        foreach (var center in biomeCenters.Values)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (Vector2Int.Distance(center, new Vector2Int(x, y)) < biomeRadius &&
                        caveTilemap.GetTile(new Vector3Int(x, y, 0)) == biomeFloorTiles[biomeCount - 1])
                    {
                        caveTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    }
                }
            }
        }
    }

    void FillEmptyTilesWithStone()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (caveTilemap.GetTile(new Vector3Int(x, y, 0)) == null && !IsInAnyBiomeRadius(new Vector2Int(x, y)) &&
                    Mathf.PerlinNoise(x * stoneNoiseScale, y * stoneNoiseScale) > stoneNoiseThreshold)
                {
                    caveTilemap.SetTile(new Vector3Int(x, y, 0), biomeFloorTiles[biomeCount - 1]);
                }
            }
        }
    }

    bool IsInAnyBiomeRadius(Vector2Int position)
    {
        foreach (var center in biomeCenters.Values)
        {
            if (Vector2Int.Distance(position, center) < biomeRadius) return true;
        }
        return false;
    }

    void FillSquareAroundBiomes()
    {
        foreach (var center in biomeCenters.Values)
        {
            int minX = Mathf.Max(0, center.x - (int)radiusSize);
            int maxX = Mathf.Min(width - 1, center.x + (int)radiusSize);
            int minY = Mathf.Max(0, center.y - (int)radiusSize);
            int maxY = Mathf.Min(height - 1, center.y + (int)radiusSize);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    radiusTilemap.SetTile(new Vector3Int(x, y, 0), radiusTile);
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
                int distanceToEdge = Mathf.Min(x, y, width - 1 - x, height - 1 - y);
                if (distanceToEdge < borderWidth)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    float noiseValue = Mathf.PerlinNoise(x * borderNoiseScale, y * borderNoiseScale);
                    if (noiseValue > borderNoiseThreshold * Mathf.InverseLerp(0, borderWidth, distanceToEdge))
                    {
                        caveTilemap.SetTile(tilePosition, borderTile);
                    }
                }
            }
        }
    }

    void CreateOutline()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Check if the current tile position is empty (open)
                if (caveTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                {
                    // Check if any neighboring tiles are filled (to create a border)
                    bool isBorderTile = false;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx != 0 || dy != 0)
                            {
                                if (caveTilemap.GetTile(new Vector3Int(x + dx, y + dy, 0)) != null)
                                {
                                    isBorderTile = true;
                                    break;
                                }
                            }
                        }
                        if (isBorderTile) break;
                    }

                    // If the tile is a border tile, set the outline tile
                    if (isBorderTile)
                    {
                        outlineTilemap.SetTile(new Vector3Int(x, y, 0), outlineTile);
                    }
                }
            }
        }
    }

    void SpawnDecorationsAboveBiomes()
    {
        foreach (var center in biomeCenters)
        {
            Vector2Int biomeCenter = center.Value;
            for (int i = 0; i < 5; i++) // Spawn 5 random decorations per biome
            {
                int xOffset = Random.Range(-5, 5);
                int yOffset = Random.Range(1, 5); // Spawn above the biome
                Vector3Int spawnPosition = new Vector3Int(biomeCenter.x + xOffset, biomeCenter.y + yOffset, 0);

                if (decorationTilemap.GetTile(spawnPosition) == null) // Only spawn if the tile is empty
                {
                    TileBase decorationTile = decorationTiles[Random.Range(0, decorationTiles.Length)];
                    decorationTilemap.SetTile(spawnPosition, decorationTile);
                }
            }
        }
    }

    void SetPlayerToCenter()
    {
        if (player != null)
        {
            player.position = new Vector3(width / 2f, height / 2f, 0f);
        }
    }

    void UpdateBiomeOverlay()
    {
        Vector2Int playerPosition = new Vector2Int(Mathf.RoundToInt(player.position.x), Mathf.RoundToInt(player.position.y));
        int newBiome = GetBiomeAtPosition(playerPosition);
        if (newBiome != currentBiome)
        {
            currentBiome = newBiome;
            DeactivateAllOverlays();
            ActivateOverlayForCurrentBiome();
        }
    }

    int GetBiomeAtPosition(Vector2Int position)
    {
        foreach (var center in biomeCenters)
        {
            if (Vector2Int.Distance(center.Value, position) <= biomeRadius)
            {
                return center.Key;
            }
        }
        return -1; // No biome found
    }

    void DeactivateAllOverlays()
    {
        foreach (var overlay in biomeOverlayPrefabs)
        {
            overlay.SetActive(false);
        }
    }

    void ActivateOverlayForCurrentBiome()
    {
        if (currentBiome >= 0 && currentBiome < biomeOverlayPrefabs.Length)
        {
            biomeOverlayPrefabs[currentBiome].SetActive(true);
        }
    }
}
