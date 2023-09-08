using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ActionIndicator : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap actionLocationTilemap;
    [SerializeField] private Tilemap actionResultTilemap;
    [SerializeField] private Tilemap intentIconTilemap;
    [SerializeField] private Tilemap intentCountTilemap;

    [Header("Tiles")]
    [SerializeField] private RuleTile highlightedTile;
    [SerializeField] private GameObject actionPreviewPrefab;
    [SerializeField] private AnimatedTile reactAnimatedTile;
    [SerializeField] private List<Tile> numberTiles;

    [Header("Settings")]
    [SerializeField] private float alpha = 0.25f;
    [SerializeField] private Color reactiveColor;
    [SerializeField] private Color delayedColor;

    private Dictionary<Vector3Int, int> threatTable;

    private void Awake()
    {
        threatTable = new Dictionary<Vector3Int, int>();
    }

    private void Start()
    {
        GameEvents.instance.onEntityMove += CheckPlayerDanger;
        GameEvents.instance.onEntityWarp += CheckPlayerDanger;
        GameEvents.instance.onEntityJump += CheckPlayerDanger;

        GameEvents.instance.onActionSelect += ShowOptions;
        GameEvents.instance.onLocationSelect += FadeOptions;
        GameEvents.instance.onActionConfirm += HideOptions;

        GameEvents.instance.onLocationSelect += DrawPath;
        GameEvents.instance.onActionThreatenLocations += ThreatenLocations;
        GameEvents.instance.onActionUnthreatenLocations += UnthreatenLocations;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onEntityMove -= CheckPlayerDanger;
        GameEvents.instance.onEntityWarp -= CheckPlayerDanger;
        GameEvents.instance.onEntityJump -= CheckPlayerDanger;

        GameEvents.instance.onActionSelect -= ShowOptions;
        GameEvents.instance.onLocationSelect -= FadeOptions;
        GameEvents.instance.onActionConfirm -= HideOptions;

        GameEvents.instance.onLocationSelect -= DrawPath;
        GameEvents.instance.onActionThreatenLocations -= ThreatenLocations;
        GameEvents.instance.onActionUnthreatenLocations -= UnthreatenLocations;
    }

    private void CheckPlayerDanger(Entity entity)
    {
        // If player is on a threatened tile
        if (entity is Player)
        {
            // Trigger proper event
            if (threatTable.ContainsKey(entity.location))
            {
                GameEvents.instance.TriggerOnEntityInDanger(entity);
            }
            else
            {
                GameEvents.instance.TriggerOnEntityOutDanger(entity);
            }
        }
    }

    private void ThreatenLocations(Action action, List<Vector3Int> locations)
    {
        // Only Show non-movment actions
        if (action.actionType != ActionType.Movement)
        {
            foreach (var location in locations)
            {
                switch (action.actionSpeed)
                {
                    case ActionSpeed.Instant:

                        // Highlight tile
                        actionResultTilemap.SetTile(location, highlightedTile);
                        actionResultTilemap.SetColor(location, Color.yellow);

                        break;
                    case ActionSpeed.Reactive:

                        // If value already is marked, then increment count
                        if (threatTable.TryGetValue(location, out int count))
                        {
                            // Update entry
                            threatTable[location] = count + 1;
                            intentCountTilemap.SetTile(location, numberTiles[count]);
                            intentCountTilemap.SetColor(location, delayedColor);
                        }
                        else
                        {
                            // Set icon
                            intentIconTilemap.SetTile(location, reactAnimatedTile);
                            intentIconTilemap.SetColor(location, reactiveColor);

                            // Add to dict
                            threatTable[location] = 1;
                        }

                        break;
                    case ActionSpeed.Delayed:

                        // If value already is marked, then increment count
                        if (threatTable.TryGetValue(location, out int count1))
                        {
                            // Update entry
                            threatTable[location] = count1 + 1;
                            intentCountTilemap.SetTile(location, numberTiles[count1]);
                            intentCountTilemap.SetColor(location, delayedColor);
                        }
                        else
                        {
                            // Set icon
                            intentIconTilemap.SetTile(location, reactAnimatedTile);
                            intentIconTilemap.SetColor(location, delayedColor);

                            // Add to dict
                            threatTable[location] = 1;

                            // Check if player is in
                            CheckPlayerDanger(DataManager.instance.GetPlayer());
                        }

                        break;
                }
            }

        }
    }

    private void UnthreatenLocations(Action action, List<Vector3Int> locations)
    {
        // Only Show non-movment actions
        if (action.actionType != ActionType.Movement)
        {
            foreach (var location in locations)
            {
                switch (action.actionSpeed)
                {
                    case ActionSpeed.Instant:

                        // unhighlight tile
                        actionResultTilemap.SetTile(location, null);

                        break;
                    case ActionSpeed.Reactive:

                        // If value already is marked, then increment count
                        if (threatTable.TryGetValue(location, out int count))
                        {
                            // Remove entry
                            if (count == 1)
                            {
                                // Unmark
                                intentIconTilemap.SetTile(location, null);

                                // Remove entry
                                threatTable.Remove(location);
                            }
                            else
                            {
                                // Update entry
                                threatTable[location] = count - 1;
                            }

                            intentCountTilemap.SetTile(location, numberTiles[count - 1]);
                        }

                        break;
                    case ActionSpeed.Delayed:

                        // If value already is marked, then increment count
                        if (threatTable.TryGetValue(location, out int count1))
                        {
                            // Remove entry
                            if (count1 == 1)
                            {
                                // Unmark
                                intentIconTilemap.SetTile(location, null);

                                // Remove entry
                                threatTable.Remove(location);

                                CheckPlayerDanger(DataManager.instance.GetPlayer());
                            }
                            else
                            {
                                // Update entry
                                threatTable[location] = count1 - 1;
                                intentCountTilemap.SetTile(location, numberTiles[count1 - 2]);
                            }
                        }

                        break;
                }
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
        var targetWorldLocation = intentIconTilemap.GetCellCenterWorld(location);
        // Instaniate as child
        var actionPreview = Instantiate(actionPreviewPrefab, targetWorldLocation, Quaternion.identity, intentIconTilemap.transform).GetComponent<ActionPreview>();
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
