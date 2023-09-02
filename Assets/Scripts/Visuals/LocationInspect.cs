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

    private void SelectTile(Vector3Int location)
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
            // print("Selected: " + location);

            // Set tile
            selectionTilemap.SetTile(location, selectionTile);

            // Inspect tile
            GameManager.instance.InspectLocation(location);
        }
        else
        {
            // Remove selection
            selectionTilemap.SetTile(location, null);
            // Deselect
            GameManager.instance.InspectLocation(location);
        }

        // Set new location
        this.selectedLocation = location;
    }
}
