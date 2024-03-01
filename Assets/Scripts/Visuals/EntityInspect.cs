using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityInspect : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private AnimatedTile selectionTile;
    [SerializeField] private Tilemap inspectTilemap;
    [SerializeField] private RuleTile highlightedTile;

    [Header("Debug")]
    [SerializeField] private Vector3Int selectedLocation;

    private void Awake()
    {
        // Set default inspect
        selectedLocation = Vector3Int.zero;
    }

    private void Start()
    {
        GameEvents.instance.onEntityInspect += Inspect;
        GameEvents.instance.onEntityDespawn += Clear;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onEntityInspect -= Inspect;
        GameEvents.instance.onEntityDespawn -= Clear;
    }

    private void LateUpdate()
    {
        if (!PauseManager.instance.IsPaused)
            HoverTile();
    }

    private void HoverTile()
    {
        // Get world position from camera
        Vector3 cameraLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cameraLocation.z = 0; // Because camera is -10 from the world
        Vector3Int selectedLocation = selectionTilemap.WorldToCell(cameraLocation);

        if (this.selectedLocation != selectedLocation && IsPositionWithinView(cameraLocation))
        {
            // Inspect tile
            GameManager.instance.InspectLocation(selectedLocation);

            // Inspect location
            this.selectedLocation = selectedLocation;
        }
    }

    private bool IsPositionWithinView(Vector3 position)
    {
        Vector3 view = Camera.main.WorldToViewportPoint(position);
        return view.x >= 0 && view.x < 1 && view.y >= 0 && view.y < 1;
    }

    private void Inspect(Entity entity, List<Vector3Int> locations)
    {
        // Clear first
        inspectTilemap.ClearAllTiles();
        selectionTilemap.ClearAllTiles();

        if (entity != null)
        {
            // Highlight tile
            selectionTilemap.SetTile(entity.location, selectionTile);

            // Highlight tiles
            if (locations != null)
                foreach (var location in locations)
                {
                    inspectTilemap.SetTile(location, highlightedTile);
                    inspectTilemap.SetTileFlags(location, TileFlags.None);
                }
        }
    }

    private void Clear(Entity entity)
    {
        if (entity.location == selectedLocation)
        {
            // Clear highlights
            inspectTilemap.ClearAllTiles();
            selectionTilemap.ClearAllTiles();
        }
    }
}
