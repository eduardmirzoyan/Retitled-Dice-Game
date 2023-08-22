using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyIntentUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap intentTilemap;
    [SerializeField] private RuleTile markTile;


    private Dictionary<Vector3Int, int> table;

    private void Awake()
    {
        table = new Dictionary<Vector3Int, int>();
    }

    private void Start()
    {
        GameEvents.instance.onEntityWatchLocation += MarkTile;
        GameEvents.instance.onEntityUnwatchLocation += UnmarkTile;

    }

    private void OnDestroy()
    {
        GameEvents.instance.onEntityWatchLocation -= MarkTile;
        GameEvents.instance.onEntityUnwatchLocation -= UnmarkTile;
    }

    private void MarkTile(Vector3Int location)
    {
        // If value already is marked, then increment count
        if (table.TryGetValue(location, out int count))
        {
            table[location] = count + 1;
        }
        else
        {
            // Mark tile
            intentTilemap.SetTile(location, markTile);
            intentTilemap.SetColor(location, Color.red);

            // Add to dict
            table[location] = 1;
        }
    }

    private void UnmarkTile(Vector3Int location)
    {
        // If value already is marked, then increment count
        if (table.TryGetValue(location, out int count))
        {
            // Remove entry
            if (count == 1)
            {
                // Unmark
                intentTilemap.SetTile(location, null);

                // Update dict
                table.Remove(location);
            }
            else
            {
                table[location] = count - 1;
            }
        }
        else
        {
            throw new System.Exception("TRIED TO UNMARK A LOCATION THAT WAS NEVER MARKED?!");
        }
    }
}
