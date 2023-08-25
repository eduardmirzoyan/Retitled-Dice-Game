using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

public class ActionIndicator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap intentionTilemap;
    [SerializeField] private Tilemap intentionIconTilemap;
    [SerializeField] private Tilemap previewTilemap;
    [SerializeField] private Tilemap inspectTilemap;
    [SerializeField] private RuleTile highlightedTile;
    [SerializeField] private GameObject actionPreviewPrefab;
    [SerializeField] private Tile reactiveIconTile;
    [SerializeField] private Tile delayedIconTile;

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
        GameEvents.instance.onLocationSelect += FocusTile;
        GameEvents.instance.onActionConfirm += HideOptions;
        GameEvents.instance.onActionThreatenLocation += ThreatenTile;
        GameEvents.instance.onActionUnthreatenLocation += UnthreatenTile;
        GameEvents.instance.onThreatsInspect += HighlightTiles;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onActionSelect -= ShowOptions;
        GameEvents.instance.onLocationSelect -= FocusTile;
        GameEvents.instance.onActionConfirm -= HideOptions;
        GameEvents.instance.onActionThreatenLocation -= ThreatenTile;
        GameEvents.instance.onActionUnthreatenLocation -= UnthreatenTile;
        GameEvents.instance.onThreatsInspect -= HighlightTiles;
    }

    private void ThreatenTile(Action action, Vector3Int location)
    {
        // If value already is marked, then increment count
        if (threatTable.TryGetValue(location, out int count))
        {
            // Update entry
            threatTable[location] = count + 1;
        }
        else
        {
            // Highlight tile
            intentionTilemap.SetTile(location, highlightedTile);
            intentionTilemap.SetColor(location, action.color);

            // Set icon
            intentionIconTilemap.SetTile(location, reactiveIconTile);
            intentionIconTilemap.SetColor(location, action.color);

            // Add to dict
            threatTable[location] = 1;
        }
    }

    private void UnthreatenTile(Action action, Vector3Int location)
    {
        // If value already is marked, then increment count
        if (threatTable.TryGetValue(location, out int count))
        {
            // Remove entry
            if (count == 1)
            {
                // Unmark
                intentionTilemap.SetTile(location, null);

                // Remove icon
                intentionIconTilemap.SetTile(location, null);

                // Remove entry
                threatTable.Remove(location);
            }
            else
            {
                // Update entry
                threatTable[location] = count - 1;
            }
        }
        else
        {
            throw new System.Exception("TRIED TO UNMARK A LOCATION THAT WAS NEVER MARKED?!");
        }
    }

    private void ShowOptions(Entity entity, Action action)
    {
        if (entity != null)
        {
            // Clear tiles first
            previewTilemap.ClearAllTiles();

            if (action != null) // If action was selected
            {
                // Get a list of all valid locations by this action
                var validLocations = action.GetValidLocations(entity.location, entity.room);

                // Display all of the action's valid locations
                foreach (var location in validLocations)
                {
                    // Set tile
                    previewTilemap.RemoveTileFlags(location, TileFlags.LockColor);
                    previewTilemap.SetTile(location, highlightedTile);
                    previewTilemap.SetColor(location, action.color);

                    // Spawn a indicator node here if it's a player
                    if (entity is Player)
                        SpawnIndicator(entity, action, location);
                }
            }
        }
    }

    private void SpawnIndicator(Entity entity, Action action, Vector3Int location)
    {
        // Get target world position
        var targetWorldLocation = intentionTilemap.GetCellCenterWorld(location);
        // Instaniate as child
        var actionPreview = Instantiate(actionPreviewPrefab, targetWorldLocation, Quaternion.identity, intentionTilemap.transform).GetComponent<ActionPreview>();
        // Initialize
        actionPreview.Initialize(entity, location, action);
    }

    private void HideOptions(Entity entity, Action action, Vector3Int location)
    {
        // Clear tiles first
        previewTilemap.ClearAllTiles();
    }

    private void FocusTile(Action action, Vector3Int location)
    {
        foreach (Vector3Int cellPosition in previewTilemap.cellBounds.allPositionsWithin)
        {
            previewTilemap.RemoveTileFlags(cellPosition, TileFlags.LockColor);

            if (previewTilemap.HasTile(cellPosition))
            {
                if (location == Vector3Int.zero)
                {
                    previewTilemap.SetColor(cellPosition, action.color);

                }
                else
                {
                    var newColor = action.color;
                    newColor.a = alpha;
                    previewTilemap.SetColor(cellPosition, newColor);
                }
            }
        }
    }

    private void HighlightTiles(List<Vector3Int> locations)
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
}
