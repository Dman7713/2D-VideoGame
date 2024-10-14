using System.Collections;
using UnityEngine;

public enum ParticleType { Empty, Sand, Water }

public class GridSystem : MonoBehaviour
{
    public int gridWidth = 50;
    public int gridHeight = 50;
    public float cellSize = 0.1f; // Size of each cell for rendering

    public ParticleType[,] grid;

    // Prefabs for visual representation
    public GameObject sandPrefab;
    public GameObject waterPrefab;

    private GameObject[,] particleVisuals; // Store visual representation

    void Start()
    {
        grid = new ParticleType[gridWidth, gridHeight];
        particleVisuals = new GameObject[gridWidth, gridHeight];
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // Initialize all cells to empty
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = ParticleType.Empty;
            }
        }
    }

    void Update()
    {
        UpdateParticles();
        RenderGrid();
    }

    void UpdateParticles()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] == ParticleType.Sand)
                {
                    UpdateSand(x, y);
                }
                else if (grid[x, y] == ParticleType.Water)
                {
                    UpdateWater(x, y);
                }
            }
        }
    }

    void UpdateSand(int x, int y)
    {
        // If the cell below is empty, move the sand down
        if (y > 0 && grid[x, y - 1] == ParticleType.Empty)
        {
            grid[x, y - 1] = ParticleType.Sand;
            grid[x, y] = ParticleType.Empty;
        }
    }

    void UpdateWater(int x, int y)
    {
        // If the cell below is empty, move the water down
        if (y > 0 && grid[x, y - 1] == ParticleType.Empty)
        {
            grid[x, y - 1] = ParticleType.Water;
            grid[x, y] = ParticleType.Empty;
        }
        // If the cell below is not empty, try to move left or right
        else if (x > 0 && grid[x - 1, y] == ParticleType.Empty)
        {
            grid[x - 1, y] = ParticleType.Water;
            grid[x, y] = ParticleType.Empty;
        }
        else if (x < gridWidth - 1 && grid[x + 1, y] == ParticleType.Empty)
        {
            grid[x + 1, y] = ParticleType.Water;
            grid[x, y] = ParticleType.Empty;
        }
    }

    void RenderGrid()
    {
        // Go through each cell and update the visual representation
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (particleVisuals[x, y] == null && grid[x, y] != ParticleType.Empty)
                {
                    // Create visual particle for sand or water
                    if (grid[x, y] == ParticleType.Sand)
                    {
                        particleVisuals[x, y] = Instantiate(sandPrefab, GetCellPosition(x, y), Quaternion.identity);
                    }
                    else if (grid[x, y] == ParticleType.Water)
                    {
                        particleVisuals[x, y] = Instantiate(waterPrefab, GetCellPosition(x, y), Quaternion.identity);
                    }
                }
                else if (particleVisuals[x, y] != null && grid[x, y] == ParticleType.Empty)
                {
                    // Destroy particle visuals if they become empty
                    Destroy(particleVisuals[x, y]);
                    particleVisuals[x, y] = null;
                }
            }
        }
    }

    Vector3 GetCellPosition(int x, int y)
    {
        return new Vector3(x * cellSize, y * cellSize, 0);
    }

    public void AddParticle(ParticleType particleType, int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            grid[x, y] = particleType;
        }
    }
}
