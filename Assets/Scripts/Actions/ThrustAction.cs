using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Thrust")]
public class ThrustAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // For each cardinal direction
        foreach (var location in room.GetNeighbors(startLocation, true))
        {
            Vector3Int direction = location - startLocation;
            result.Add(room.GetFirstValidLocationWithinRange(startLocation, direction, die.value));
        }

        return result;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        return entity.room.GetAllValidLocationsAlongPath(entity.location, targetLocation, true);
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        foreach (var location in threatenedLocations)
        {
            var target = room.GetEntityAtLocation(location);
            if (target != null)
            {
                entity.Attack(target);
            }
        }

        // Trigger event
        GameEvents.instance.TriggerOnEntityUseWeapon(entity, weapon);

        // Wait for animation
        yield return new WaitForSeconds(GameManager.instance.gameSettings.weaponMeleeBufferTime);
    }
}
