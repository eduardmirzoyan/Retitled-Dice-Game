using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Piercing Ranged")]
public class PiercingRangedAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        // Search in each direction
        foreach (var direction in cardinalDirections)
        {
            var location = startLocation + direction;
            var range = die.value;

            // Check range
            while (range > 0)
            {
                if (room.IsWall(location))
                {
                    targets.Add(location - direction);

                    break;
                }

                location += direction;
                range--;

                if (range == 0)
                {
                    targets.Add(location);
                }
            }
        }

        return targets;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        Vector3Int start = entity.location;
        Vector3Int end = targetLocation;

        Vector3Int direction = end - start;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        while (start != end)
        {
            // Increment start
            start += direction;

            // Check to see if the location is valid
            if (entity.room.IsWall(start))
            {
                break;
            }

            result.Add(start);
        }

        return result;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // Logic
        foreach (var location in threatenedLocations)
        {
            // Damage first target found
            var target = room.GetEntityAtLocation(location);
            if (target != null)
            {
                // Damage location
                entity.Attack(target);

                // Dip
                break;
            }
        }

        // Trigger event
        GameEvents.instance.TriggerOnEntityUseWeapon(entity, weapon);

        yield return null;
    }
}
