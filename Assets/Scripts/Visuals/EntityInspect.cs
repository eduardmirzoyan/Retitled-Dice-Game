using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityInspect : MonoBehaviour
{
    // TODO FIX THIS!!!!!!!!!!!!!!!!!

    [Header("Static Data")]
    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private AnimatedTile selectionTile;

    [Header("Dynamic Data")]
    [SerializeField] private Vector3Int selectedLocation;

    private void Awake()
    {
        // Set default inspect
        selectedLocation = Vector3Int.back;
    }

    private void Start()
    {
        GameEvents.instance.onEntityInspect += InspectLocations;

        //GameEvents.instance.onEntityInspect += HighlightLocations;
        //GameEvents.instance.onEntityDespawn += ClearLocations;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onEntityInspect -= InspectLocations;

        //GameEvents.instance.onEntityInspect -= HighlightLocations;
        //GameEvents.instance.onEntityDespawn -= ClearLocations;
    }

    private void Update()
    {
        HoverTile();
    }

    private void HoverTile()
    {
        // Get world position from camera
        Vector3 cameraLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cameraLocation.z = 0; // Because camera is -10 from the world
        Vector3Int selectedLocation = selectionTilemap.WorldToCell(cameraLocation);

        if (this.selectedLocation != selectedLocation)
        {
            // Inspect tile
            GameManager.instance.InspectLocation(selectedLocation);

            // Inspect location
            this.selectedLocation = selectedLocation;
        }
    }

    private void InspectLocations(Entity entity, List<Vector3Int> list)
    {
        // Highlight tile
        selectionTilemap.SetTile(entity.location, selectionTile);


    }
}
