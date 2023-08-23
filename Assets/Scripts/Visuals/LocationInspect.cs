using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LocationInspect : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private AnimatedTile selectionTile;

    [Header("Debug")]
    [SerializeField] private Vector3Int selectedLocation;

    private void Awake()
    {
        // Set default inspect
        selectedLocation = Vector3Int.back;
    }

    private void Update()
    {
        // Left click to make selection
        if (Input.GetMouseButtonDown(0))
        {
            // Get world position from camera
            Vector3 cameraLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cameraLocation.z = 0; // Because camera is -10 from the world
            Vector3Int selectedLocation = selectionTilemap.WorldToCell(cameraLocation);

            // Select position
            SelectTile(selectedLocation);
        }
        // Right click clears any selection
        else if (Input.GetMouseButtonDown(1))
        {
            // Clear selection
            SelectTile(Vector3Int.back);
        }
    }

    public void SelectTile(Vector3Int location)
    {
        // If selected tile is not undefined 
        if (selectedLocation != Vector3Int.back)
        {
            // Clear last selection
            selectionTilemap.SetTile(selectedLocation, null);
        }

        // If new location isn't empty
        if (location != Vector3Int.back)
        {
            // Debug
            print("Selected: " + location);

            // Set tile
            selectionTilemap.SetTile(location, selectionTile);

            // Inspect tile
            InspectLocation(location);
        }
        else
        {
            // Remove selection
            selectionTilemap.SetTile(location, null);
            // Deselect
            InspectLocation(location);
        }

        // Set new location
        this.selectedLocation = location;
    }

    private void InspectLocation(Vector3Int location)
    {
        // // Look for enemy on this tile
        // foreach (var enemy in room.hostileEntities)
        // {
        //     // If an enemy exists at this location
        //     if (enemy.location == location)
        //     {
        //         // Trigger inspect event
        //         GameEvents.instance.TriggerOnEntityInspect(enemy);
        //         // Finish
        //         return;
        //     }
        // }

        // // Look at barrels
        // foreach (var barrel in room.neutralEntities)
        // {
        //     // If an enemy exists at this location
        //     if (barrel.location == location)
        //     {
        //         // Trigger inspect event
        //         GameEvents.instance.TriggerOnEntityInspect(barrel);
        //         // Finish
        //         return;
        //     }
        // }

        // // Else if enemy was not found, then remove any previous inspect
        // GameEvents.instance.TriggerOnEntityInspect(null);
    }
}
