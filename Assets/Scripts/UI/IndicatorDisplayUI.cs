using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IndicatorDisplayUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap floorTilemap;

    [Header("Data")]
    [SerializeField] private GameObject indicatorUIPrefab;

    private List<LocationIndicatorUI> locationIndicatorUIs;

    private void Start()
    {
        // Sub
        GameEvents.instance.onActionSelect += SpawnIndicators;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onActionSelect -= SpawnIndicators;
    }

    private void SpawnIndicators(Entity entity, Action action, Room room)
    {
        if (action != null)
        {
            // Get a list of all valid locations by this action
            var validLocations = action.GetValidLocations(entity.location, room);

            // Get entity location
            var entityWorldLocation = floorTilemap.GetCellCenterWorld(entity.location);

            // Display all of the action's valid locations
            foreach (var targetLocation in validLocations)
            {
                // Get world position
                var targetWorldLocation = floorTilemap.GetCellCenterWorld(targetLocation);
                // Instaniate as child
                var indicatorUI = Instantiate(indicatorUIPrefab, targetWorldLocation, Quaternion.identity, transform).GetComponent<LocationIndicatorUI>();

                // Initialize
                indicatorUI.Initialize(entity, targetLocation, entityWorldLocation, targetWorldLocation, action);
            }
        }

    }
}
