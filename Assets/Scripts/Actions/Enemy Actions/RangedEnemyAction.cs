using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Enemy/Ranged")]
public class RangedEnemyAction : Action
{
    private Entity watcher;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // For each cardinal direction
        foreach (var location in room.GetNeighbors(startLocation, true))
        {
            Vector3Int direction = location - startLocation;
            result.Add(room.GetFirstValidLocation(startLocation, direction, true));
        }

        return result;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Room room)
    {
        // Give small buffer time
        yield return new WaitForSeconds(0.1f);

        // Watch the tiles in a line between end points
        threatenedLocations = room.GetAllValidLocationsAlongPath(entity.location, targetLocation, true);
        watcher = entity;

        // Trigger events
        foreach (var location in threatenedLocations)
        {
            GameEvents.instance.TriggerOnActionThreatenLocation(location);
        }

        // Sub to events
        GameEvents.instance.onEntityEnterTile += Retaliate;
        GameEvents.instance.onTurnStart += DisableRetaliation;

        // Pull out weapon
        Vector3Int direction = targetLocation - entity.location;
        GameEvents.instance.TriggerOnEntityDrawWeapon(watcher, direction, weapon);
    }

    private void Retaliate(Entity entity, Vector3Int location)
    {
        // If player entered tile that is being watched
        if (entity is Player && threatenedLocations.Contains(location))
        {
            // Attack entity
            // CHANGE THIS TO RANGED?!
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
        foreach (var location in threatenedLocations)
        {
            GameEvents.instance.TriggerOnActionUnthreatenLocation(location);
        }

        // Sheathe weapon
        GameEvents.instance.TriggerOnEntitySheatheWeapon(watcher, weapon);

        // Reset data
        threatenedLocations = null;
        watcher = null;

        // Unsub from events
        GameEvents.instance.onEntityEnterTile -= Retaliate;
        GameEvents.instance.onTurnStart -= DisableRetaliation;
    }
}
