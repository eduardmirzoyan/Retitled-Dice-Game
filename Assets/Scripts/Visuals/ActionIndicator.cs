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

    [Header("Settings")]
    [SerializeField] private float alpha = 0.25f;

    private Dictionary<Vector3Int, int> attackIntentTable;
    private Dictionary<Vector3Int, int> utilityIntentTable;

    private void Awake()
    {
        attackIntentTable = new Dictionary<Vector3Int, int>();
        utilityIntentTable = new Dictionary<Vector3Int, int>();
    }

    private void Start()
    {
        GameEvents.instance.onActionSelect += ShowOptions;
        GameEvents.instance.onLocationSelect += FadeOptions;
        GameEvents.instance.onActionConfirm += HideOptions;

        GameEvents.instance.onLocationSelect += DrawPath;
        GameEvents.instance.onActionThreatenLocations += ThreatenLocations;
        GameEvents.instance.onActionUnthreatenLocations += UnthreatenLocations;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onActionSelect -= ShowOptions;
        GameEvents.instance.onLocationSelect -= FadeOptions;
        GameEvents.instance.onActionConfirm -= HideOptions;

        GameEvents.instance.onLocationSelect -= DrawPath;
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

                                // // Create indicator
                                // var position = intentTilemap.GetCellCenterWorld(location);
                                // var damageIndicator = Instantiate(damageIntentPrefab, position, Quaternion.identity, intentTilemap.transform).GetComponent<DamageIntentIndicator>();
                                // damageIndicator.Initialize(damage);

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

    private void ShowOptions(Entity entity, Action action)
    {
        if (entity != null)
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

                    // Spawn a indicator node here if it's a player
                    if (entity is Player)
                        SpawnIndicator(entity, action, location);
                }
            }
        }
    }

    private void FadeOptions(Entity entity, Action action, Vector3Int location)
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

    private void HideOptions(Entity entity, Action action, Vector3Int location)
    {
        // Clear tiles first
        actionLocationTilemap.ClearAllTiles();
    }

    private void SpawnIndicator(Entity entity, Action action, Vector3Int location)
    {
        // Get target world position
        var targetWorldLocation = intentTilemap.GetCellCenterWorld(location);
        // Instaniate as child
        var actionPreview = Instantiate(actionPreviewPrefab, targetWorldLocation, Quaternion.identity, intentTilemap.transform).GetComponent<ActionPreview>();
        // Initialize
        actionPreview.Initialize(entity, location, action);
    }

    private void DrawPath(Entity entity, Action action, Vector3Int location)
    {
        if (location != Vector3Int.back && action.pathPrefab != null)
        {
            var offset = new Vector3(0.5f, 0.5f, -1);
            Instantiate(action.pathPrefab, transform).GetComponent<ActionPathRenderer>().Initialize(entity, action, location, entity.location + offset, location + offset, action.color);
        }
    }
}
