using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IndicatorDisplayUI : MonoBehaviour
{
    [SerializeField] private GameObject indicatorUIPrefab;

    private List<LocationIndicatorUI> locationIndicatorUIs;

    private void Start()
    {
        locationIndicatorUIs = new List<LocationIndicatorUI>();

        // Sub
        GameEvents.instance.onActionSelect += SpawnIndicators;
        GameEvents.instance.onLocationSelect += DespawnIndicators;
    }

    private void OnDestroy() {
        // Unsub
        GameEvents.instance.onActionSelect -= SpawnIndicators;
        GameEvents.instance.onLocationSelect -= DespawnIndicators;
    }

    private void SpawnIndicators(Entity entity, Action action, Room room)
    {
        if (action != null) {
            // Get a list of all valid locations by this action
            var validLocations = action.GetValidLocations(entity.location, room);

            // Display all of the action's valid locations
            foreach (var targetLocation in validLocations)
            {
                // Get world position from UI
                var worldLocation = RoomUI.instance.floorTilemap.GetCellCenterWorld(targetLocation);
                // Instaniate as child
                var indicatorUI = Instantiate(indicatorUIPrefab, worldLocation, Quaternion.identity, transform).GetComponent<LocationIndicatorUI>();
                
                // Initialize
                indicatorUI.Initialize(entity.location, targetLocation, action);
                // Save
                locationIndicatorUIs.Add(indicatorUI);
            }
        }
        else {
            // Despawn
            DespawnIndicators(Vector3Int.zero);
        }
    }

    private void DespawnIndicators(Vector3Int location)
    {
        // Loop through all indicators
        foreach (var indicatorUI in locationIndicatorUIs)
        {
            // Destroy gameobject
            Destroy(indicatorUI.gameObject);
        }

        // Clear list
        locationIndicatorUIs.Clear();
    }
}
