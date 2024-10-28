using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField] private Item item;

    [SerializeField] private TileBase highlightTile;
    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private Tilemap tempTilemap;

    private Vector3Int highlightedTilePos;
    private bool highlighted;

    private void Update()
    {
        if (item != null)
        {
            HighlightTile(item);
        }
    }

    private Vector3Int GetMouseOnGridPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseCellPos = mainTilemap.WorldToCell(mousePos);
        mouseCellPos.z = 0;

        return mouseCellPos;
    }

    private void HighlightTile(Item currentItem)
    {
        Vector3Int mouseGridPos = GetMouseOnGridPos();

        if (highlightedTilePos != mouseGridPos)
        {
            tempTilemap.SetTile(highlightedTilePos, null);

            TileBase tile = mainTilemap.GetTile(mouseGridPos);

            if (tile)
            {
                tempTilemap.SetTile(mouseGridPos, highlightTile);
                highlightedTilePos = mouseGridPos;

                highlighted = true;
            }
            else
            {
                highlighted = false;
            }
        }
    }
}