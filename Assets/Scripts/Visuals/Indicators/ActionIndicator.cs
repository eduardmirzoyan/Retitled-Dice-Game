using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ActionIndicator : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap actionLocationTilemap;
    [SerializeField] private Tilemap actionPreviewTilemap;
    [SerializeField] private Tilemap intentTilemap;

    [Header("Tiles")]
    [SerializeField] private RuleTile highlightedTile;
    [SerializeField] private AnimatedTile intentTile;

    [Header("Prefabs")]
    [SerializeField] private GameObject actionPreviewPrefab;
    [SerializeField] private GameObject damageIntentPrefab;
    [SerializeField] private GameObject directionIndicatorPrefab;

    [Header("Settings")]
    [SerializeField] private float alpha = 0.25f;

    private Dictionary<Vector3Int, int> attackIntentTable;
    private Dictionary<Vector3Int, int> utilityIntentTable;

    private List<DamageIntentIndicator> indicators;

    private void Awake()
    {
        attackIntentTable = new Dictionary<Vector3Int, int>();
        utilityIntentTable = new Dictionary<Vector3Int, int>();
        indicators = new List<DamageIntentIndicator>();
    }

    private void Start()
    {
        GameEvents.instance.onActionSelect += OnActionSelect;
        GameEvents.instance.onLocationSelect += OnLocationSelect;
        GameEvents.instance.onActionConfirm += OnActionConfirm;


        GameEvents.instance.onActionThreatenLocations += ThreatenLocations;
        GameEvents.instance.onActionUnthreatenLocations += UnthreatenLocations;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onActionSelect -= OnActionSelect;
        GameEvents.instance.onLocationSelect -= OnLocationSelect;
        GameEvents.instance.onActionConfirm -= OnActionConfirm;

        GameEvents.instance.onActionThreatenLocations -= ThreatenLocations;
        GameEvents.instance.onActionUnthreatenLocations -= UnthreatenLocations;
    }

    private void ThreatenLocations(Action action, List<Vector3Int> locations)
    {
        // Parse based on type
        switch (action.actionType)
        {
            case ActionType.Attack:

                int damage = action.GetTotalDamage();

                switch (action.actionSpeed)
                {
                    case ActionSpeed.Instant: // Instant Attack

                        foreach (var location in locations)
                        {
                            actionPreviewTilemap.SetTile(location, highlightedTile);
                            actionPreviewTilemap.SetColor(location, Color.yellow);

                            // Create indicator
                            var position = intentTilemap.GetCellCenterWorld(location);
                            var damageIndicator = Instantiate(damageIntentPrefab, position, Quaternion.identity, intentTilemap.transform).GetComponent<DamageIntentIndicator>();
                            damageIndicator.Initialize(damage);
                            damageIndicator.SetHighlightState(true);
                            indicators.Add(damageIndicator);
                        }

                        break;
                    case ActionSpeed.Delayed: // Delayed Attack

                        foreach (var location in locations)
                        {
                            // If value already is marked, then increment count
                            if (attackIntentTable.TryGetValue(location, out int value))
                            {
                                // Update entry
                                // int value = indicator.GetValue();
                                // indicator.SetValue(value + damage);
                                attackIntentTable[location] = value + damage;
                            }
                            else
                            {
                                // Set icon
                                intentTilemap.SetTile(location, intentTile);
                                intentTilemap.SetColor(location, action.color);

                                // Add to dict
                                attackIntentTable[location] = damage;
                            }
                        }

                        break;
                }

                break;
            case ActionType.Utility:

                switch (action.actionSpeed)
                {
                    case ActionSpeed.Instant: // Instant Utility

                        foreach (var location in locations)
                        {
                            actionPreviewTilemap.SetTile(location, highlightedTile);
                            actionPreviewTilemap.SetColor(location, Color.yellow);
                        }

                        break;
                    case ActionSpeed.Delayed: // Delayed Utility

                        foreach (var location in locations)
                        {
                            // If value already is marked, then increment count
                            if (utilityIntentTable.TryGetValue(location, out int count))
                            {
                                // Update entry
                                utilityIntentTable[location] = count + 1;
                            }
                            else
                            {
                                // Add to dict
                                utilityIntentTable[location] = 1;

                                // Set icon
                                intentTilemap.SetTile(location, intentTile);
                                intentTilemap.SetColor(location, action.color);
                            }
                        }

                        break;
                }

                break;
        }
    }

    private void UnthreatenLocations(Action action, List<Vector3Int> locations)
    {
        // Parse based on type
        switch (action.actionType)
        {
            case ActionType.Attack:

                int damage = action.GetTotalDamage();

                switch (action.actionSpeed)
                {
                    case ActionSpeed.Instant: // Instant Attack

                        foreach (var location in locations)
                        {
                            actionPreviewTilemap.SetTile(location, null);
                        }

                        // Clear indicator
                        foreach (var indicator in indicators)
                        {
                            Destroy(indicator.gameObject);
                        }
                        indicators.Clear();

                        break;
                    case ActionSpeed.Delayed: // Delayed Attack

                        foreach (var location in locations)
                        {
                            // If value already is marked, then increment count
                            if (attackIntentTable.TryGetValue(location, out int value))
                            {
                                // Remove entry
                                if (damage >= value)
                                {
                                    // Unmark
                                    intentTilemap.SetTile(location, null);

                                    // Destroy object
                                    // Destroy(indicator.gameObject);

                                    // Remove entry
                                    attackIntentTable.Remove(location);
                                }
                                else
                                {
                                    // Update entry
                                    // var damageIndicator = attackIntentTable[location];
                                    // int value = damageIndicator.GetValue();
                                    // damageIndicator.SetValue(value - damage);
                                    attackIntentTable[location] = value - damage;
                                }
                            }
                        }

                        break;
                }

                break;
            case ActionType.Utility:

                switch (action.actionSpeed)
                {
                    case ActionSpeed.Instant: // Instant Utility

                        foreach (var location in locations)
                        {
                            actionPreviewTilemap.SetTile(location, null);
                        }

                        break;
                    case ActionSpeed.Delayed: // Delayed Utility

                        foreach (var location in locations)
                        {
                            // If value already is marked, then increment count
                            if (utilityIntentTable.TryGetValue(location, out int count))
                            {
                                // Remove entry
                                if (count == 1)
                                {
                                    // Unmark
                                    intentTilemap.SetTile(location, null);

                                    // Remove entry
                                    utilityIntentTable.Remove(location);
                                }
                                else
                                {
                                    // Update entry
                                    utilityIntentTable[location] = count - 1;
                                }
                            }
                        }

                        break;
                }

                break;
        }
    }

    private void OnActionSelect(Entity entity, Action action)
    {
        if (entity is Player)
        {
            // Clear tiles first
            actionLocationTilemap.ClearAllTiles();

            if (action != null) // If action was selected
            {
                // Get a list of all valid locations by this action
                var validLocations = action.GetValidLocations(entity.location, entity.room);

                // Display all of the action's valid locations
                foreach (var location in validLocations)
                {
                    // Set tile
                    actionLocationTilemap.RemoveTileFlags(location, TileFlags.LockColor);
                    actionLocationTilemap.SetTile(location, highlightedTile);
                    actionLocationTilemap.SetColor(location, action.color);

                    // Spawn preview
                    var targetWorldLocation = intentTilemap.GetCellCenterWorld(location);
                    var actionPreview = Instantiate(actionPreviewPrefab, targetWorldLocation, Quaternion.identity, intentTilemap.transform).GetComponent<ActionPreview>();
                    actionPreview.Initialize(entity, location, action);
                }
            }
        }
    }

    private void OnLocationSelect(Entity entity, Action action, Vector3Int location)
    {
        if (entity is Player)
        {
            foreach (Vector3Int cellPosition in actionLocationTilemap.cellBounds.allPositionsWithin)
            {
                actionLocationTilemap.RemoveTileFlags(cellPosition, TileFlags.LockColor);

                if (actionLocationTilemap.HasTile(cellPosition))
                {
                    if (location == Vector3Int.back)
                    {
                        actionLocationTilemap.SetColor(cellPosition, action.color);
                    }
                    else
                    {
                        var newColor = action.color;
                        newColor.a = alpha;
                        actionLocationTilemap.SetColor(cellPosition, newColor);
                    }
                }
            }
        }

        // Attempt to draw path
        if (location != Vector3Int.back && action.pathPrefab != null)
        {
            var offset = new Vector3(0.5f, 0.5f, -1);
            Instantiate(action.pathPrefab, transform).GetComponent<ActionPathRenderer>().Initialize(entity, action, location, entity.location + offset, location + offset, action.color);
        }
    }

    private void OnActionConfirm(Entity entity, Action action, Vector3Int location)
    {
        if (entity is Player)
        {
            // Clear tiles first
            actionLocationTilemap.ClearAllTiles();
        }
        else
        {
            if (action.actionType == ActionType.Attack)
            {
                // Spawn directional indicator
                Vector3Int direction = location - entity.location;
                direction.Clamp(-Vector3Int.one, Vector3Int.one);

                var worldPosition = intentTilemap.GetCellCenterWorld(entity.location);
                var indicator = Instantiate(directionIndicatorPrefab, worldPosition, Quaternion.identity, intentTilemap.transform).GetComponent<DirectionIndicator>();
                indicator.Initialize(entity, direction);
            }
        }
    }
}
