using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ActionIndicator : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap actionLocationTilemap;
    [SerializeField] private Tilemap actionResultTilemap;
    [SerializeField] private Tilemap intentTilemap;
    [SerializeField] private Tilemap intentIconTilemap;
    [SerializeField] private Tilemap inspectTilemap;

    [Header("Tiles")]
    [SerializeField] private RuleTile highlightedTile;
    [SerializeField] private GameObject actionPreviewPrefab;
    [SerializeField] private AnimatedTile reactAnimatedTile;

    [Header("Settings")]
    [SerializeField] private float alpha = 0.25f;

    private Dictionary<Vector3Int, int> threatTable;

    private void Awake()
    {
        threatTable = new Dictionary<Vector3Int, int>();
    }

    private void Start()
    {
        GameEvents.instance.onActionSelect += ShowOptions;
        GameEvents.instance.onLocationSelect += FadeOptions;
        GameEvents.instance.onActionConfirm += HideOptions;

        GameEvents.instance.onLocationSelect += DrawPath;
        GameEvents.instance.onActionThreatenLocation += ThreatenTile;
        GameEvents.instance.onActionUnthreatenLocation += UnthreatenTile;

        GameEvents.instance.onThreatsInspect += OutlineThreats;
        GameEvents.instance.onEntityDespawn += ClearOutline;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onActionSelect -= ShowOptions;
        GameEvents.instance.onLocationSelect -= FadeOptions;
        GameEvents.instance.onActionConfirm -= HideOptions;

        GameEvents.instance.onLocationSelect -= DrawPath;
        GameEvents.instance.onActionThreatenLocation -= ThreatenTile;
        GameEvents.instance.onActionUnthreatenLocation -= UnthreatenTile;

        GameEvents.instance.onThreatsInspect -= OutlineThreats;
        GameEvents.instance.onEntityDespawn -= ClearOutline;
    }

    private void ThreatenTile(Action action, Vector3Int location)
    {
        // Only Show non-movment actions
        if (action.actionType != ActionType.Movement)
        {
            // If action is not immediate, then use different visuals
            if (action.actionSpeed != ActionSpeed.Instant)
            {
                // If value already is marked, then increment count
                if (threatTable.TryGetValue(location, out int count))
                {
                    // Update entry
                    threatTable[location] = count + 1;
                }
                else
                {
                    // Set icon
                    intentIconTilemap.SetTile(location, reactAnimatedTile);
                    intentIconTilemap.SetColor(location, action.color);

                    // Add to dict
                    threatTable[location] = 1;
                }
            }
            else
            {
                // Highlight tile
                actionResultTilemap.SetTile(location, highlightedTile);
                actionResultTilemap.SetColor(location, Color.yellow);
            }
        }
    }

    private void UnthreatenTile(Action action, Vector3Int location)
    {
        // Only Show non-movment actions
        if (action.actionType != ActionType.Movement)
        {
            // If action is not immediate, then use different visuals
            if (action.actionSpeed != ActionSpeed.Instant)
            {
                // If value already is marked, then increment count
                if (threatTable.TryGetValue(location, out int count))
                {
                    // Remove entry
                    if (count == 1)
                    {
                        // Unmark
                        intentTilemap.SetTile(location, null);
                        intentIconTilemap.SetTile(location, null);

                        // Remove entry
                        threatTable.Remove(location);
                    }
                    else
                    {
                        // Update entry
                        threatTable[location] = count - 1;
                    }
                }
            }
            else
            {
                // Highlight tile
                actionResultTilemap.SetTile(location, null);
            }
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
                if (location == Vector3Int.zero)
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
        if (location != Vector3Int.zero && action.pathPrefab != null)
        {
            var offset = new Vector3(0.5f, 0.5f, -1);
            Instantiate(action.pathPrefab, transform).GetComponent<ActionPathRenderer>().Initialize(entity, action, location, entity.location + offset, location + offset, action.color);
        }
    }

    private void OutlineThreats(List<Vector3Int> locations)
    {
        if (locations != null)
        {
            foreach (var location in locations)
            {
                inspectTilemap.SetTile(location, highlightedTile);
                inspectTilemap.SetTileFlags(location, TileFlags.None);
            }
        }
        else
        {
            inspectTilemap.ClearAllTiles();
        }
    }

    private void ClearOutline(Entity entity)
    {
        // TODO?
    }
}
