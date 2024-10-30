using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class CaveShadowCaster : MonoBehaviour
{
    public Tilemap caveTilemap; // Assign your tilemap here
    public GameObject shadowCasterPrefab; // Prefab with ShadowCaster2D

    void Start()
    {
        CreateShadowCasters();
    }

    void CreateShadowCasters()
    {
        foreach (var position in caveTilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = caveTilemap.GetTile(position);
            if (tile != null)
            {
                Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
                Vector3 worldPosition = caveTilemap.GetCellCenterWorld(tilePosition);

                // Instantiate shadow caster for each tile
                GameObject shadowCaster = Instantiate(shadowCasterPrefab, worldPosition, Quaternion.identity, transform);

                // Optional: Configure shadow caster settings if needed
                ShadowCaster2D shadowCaster2D = shadowCaster.GetComponent<ShadowCaster2D>();
                shadowCaster2D.selfShadows = true; // or false based on your needs
                // Do not attempt to modify shadow shape here
            }
        }
    }
}
