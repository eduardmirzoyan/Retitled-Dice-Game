using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityInspect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private Tilemap inspectTilemap;

    [Header("Data")]
    [SerializeField] private AnimatedTile selectionTile;
    [SerializeField] private RuleTile highlightedTile;
    [SerializeField] private GameObject damageIntentPrefab;

    [Header("Debug")]
    [SerializeField] private Vector3Int selectedLocation;
    [SerializeField] private List<DamageIndicator> indicators;

    private void Awake()
    {
        // Set default inspect
        selectedLocation = Vector3Int.zero;
        indicators = new List<DamageIndicator>();
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

    private void Inspect(Entity entity, Action action, List<Vector3Int> targetLocations)
    {
        // Clear first
        ClearSelection();

        if (entity != null)
        {
            // Highlight entity
            selectionTilemap.SetTile(entity.location, selectionTile);

            foreach (var location in targetLocations)
            {
                // Outline
                inspectTilemap.SetTile(location, highlightedTile);

                // Display damage if atttack
                if (action.actionType == ActionType.Attack)
                {
                    int damage = action.GetTotalDamage();

                    // Spawn indicator
                    var position = inspectTilemap.GetCellCenterWorld(location);
                    var damageIndicator = Instantiate(damageIntentPrefab, position, Quaternion.identity, inspectTilemap.transform).GetComponent<DamageIndicator>();
                    damageIndicator.Initialize(damage, action.color);

                    // Store reference
                    indicators.Add(damageIndicator);
                }
                else if (action.actionType == ActionType.Utility)
                {
                    // Spawn indicator
                    var position = inspectTilemap.GetCellCenterWorld(location);
                    var damageIndicator = Instantiate(damageIntentPrefab, position, Quaternion.identity, inspectTilemap.transform).GetComponent<DamageIndicator>();
                    damageIndicator.Initialize("$", action.color);

                    // Store reference
                    indicators.Add(damageIndicator);
                }
            }
        }
    }

    private void Clear(Entity entity)
    {
        if (entity.location == selectedLocation)
        {
            ClearSelection();
        }
    }

    private void ClearSelection()
    {
        // Clear tiles
        inspectTilemap.ClearAllTiles();
        selectionTilemap.ClearAllTiles();

        // Clear indicators
        foreach (var indicator in indicators)
        {
            Destroy(indicator.gameObject);
        }
        indicators.Clear();
    }
}
