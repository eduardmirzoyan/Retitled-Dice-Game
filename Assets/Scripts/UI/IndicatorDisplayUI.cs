using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

public class IndicatorDisplayUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap selectTilemap;

    [Header("Data")]
    [SerializeField] private GameObject indicatorUIPrefab;
    [SerializeField] private RuleTile highlightTile;

    private void Start()
    {
        // Sub
        GameEvents.instance.onActionSelect += DisplayChoices;
        GameEvents.instance.onLocationSelect += ClearChoices;
        GameEvents.instance.onInspectAction += PreviewIndicators;
        GameEvents.instance.onEntityWatchLocation += HighlightTile;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onActionSelect -= DisplayChoices;
        GameEvents.instance.onLocationSelect -= ClearChoices;
        GameEvents.instance.onInspectAction -= PreviewIndicators;
        GameEvents.instance.onEntityWatchLocation += HighlightTile;
    }

    private void SpawnIndicator(Entity entity, Action action, Vector3Int location)
    {
        // Get entity's world position
        var entityWorldLocation = selectTilemap.GetCellCenterWorld(entity.location);
        // Get target world position
        var targetWorldLocation = selectTilemap.GetCellCenterWorld(location);
        // Instaniate as child
        var indicatorUI = Instantiate(indicatorUIPrefab, targetWorldLocation, Quaternion.identity, transform).GetComponent<LocationIndicatorUI>();
        // Initialize
        indicatorUI.Initialize(entity, location, entityWorldLocation, targetWorldLocation, action);
    }

    private void DisplayChoices(Entity entity, Action action)
    {
        if (entity != null)
        {
            // Clear tiles first
            selectTilemap.ClearAllTiles();

            if (action != null) // If action was selected
            {
                // Get a list of all valid locations by this action
                var validLocations = action.GetValidLocations(entity.location, entity.room);

                // Display all of the action's valid locations
                foreach (var location in validLocations)
                {
                    // Set tile
                    selectTilemap.SetTile(location, highlightTile);
                    selectTilemap.SetColor(location, action.color);

                    // Spawn a indicator node here
                    SpawnIndicator(entity, action, location);
                }
            }
        }
    }

    private void ClearChoices(Entity entity, Action action, Vector3Int location)
    {
        // Clear tiles first
        selectTilemap.ClearAllTiles();
    }

    private void PreviewIndicators(Entity entity, Action action)
    {
        if (entity != null && action != null)
        {
            // Get a list of all valid locations by this action
            var validLocations = action.GetValidLocations(entity.location, entity.room);

            // Get entity location
            var entityWorldLocation = selectTilemap.GetCellCenterWorld(entity.location);

            // Display all of the action's valid locations
            foreach (var location in validLocations)
            {
                // Get world position
                var targetWorldLocation = selectTilemap.GetCellCenterWorld(location);
                // Instaniate as child
                var indicatorUI = Instantiate(indicatorUIPrefab, targetWorldLocation, Quaternion.identity, transform).GetComponent<LocationIndicatorUI>();
                // Initialize
                indicatorUI.Initialize(entity, location, entityWorldLocation, targetWorldLocation, action, true);
            }
        }
    }

    private void HighlightTile(Entity entity, Vector3Int location)
    {
        // print("Threaened: " + location);

        // Set tile to color
        selectTilemap.SetTile(location, highlightTile);
        selectTilemap.SetColor(location, Color.red);
    }
}
