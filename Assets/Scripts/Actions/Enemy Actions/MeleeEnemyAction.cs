using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[CreateAssetMenu(menuName = "Actions/Enemy/Melee")]
public class MeleeEnemyAction : EnemyAction
{
    public List<Vector3Int> tilesToWatch;
    private Entity watcher;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        return room.GetNeighbors(startLocation, true);
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Room room)
    {
        // Mathematically calculate side tiles
        Vector3Int direction = targetLocation - entity.location;
        Vector3Int side1 = Vector3Int.RoundToInt(Vector3.Cross(direction, Vector3Int.forward));
        Vector3Int side2 = Vector3Int.RoundToInt(Vector3.Cross(direction, Vector3Int.back));

        // Watch the tiles left and right of target location if valid
        tilesToWatch = new List<Vector3Int>() { targetLocation, targetLocation + side1, targetLocation + side2 };
        watcher = entity;

        // Trigger events
        GameEvents.instance.TriggerOnEntityWatchLocation(targetLocation);
        GameEvents.instance.TriggerOnEntityWatchLocation(targetLocation + side1);
        GameEvents.instance.TriggerOnEntityWatchLocation(targetLocation + side2);

        // Sub to events
        GameEvents.instance.onEntityEnterTile += Retaliate;
        GameEvents.instance.onTurnStart += DisableRetaliation;

        // Pull out weapon
        GameEvents.instance.TriggerOnEntityDrawWeapon(watcher, direction, weapon);

        // Gamemanger should handle this wait time
        yield return new WaitForSeconds(0.5f);
    }

    private void Retaliate(Entity entity, Vector3Int location)
    {
        // If player entered tile that is being watched
        if (entity is Player && tilesToWatch.Contains(location))
        {
            // Attack entity
            watcher.MeleeAttackEntity(entity, weapon);

            Reset();
        }
    }

    private void DisableRetaliation(Entity entity)
    {
        // If the watcher's turn is started
        if (entity == watcher)
        {
            Reset();
        }
    }

    private void Reset()
    {
        // Unhighlight tiles
        foreach (var location in tilesToWatch)
        {
            GameEvents.instance.TriggerOnEntityUnwatchLocation(location);
        }

        // Sheathe weapon
        // GameEvents.instance.TriggerOnEntitySheatheWeapon(watcher, weapon);
        // FIGURE THIS OUT?!

        // Reset data
        tilesToWatch = null;
        watcher = null;

        // Unsub from events
        GameEvents.instance.onEntityEnterTile -= Retaliate;
        GameEvents.instance.onTurnStart -= DisableRetaliation;
    }
}
