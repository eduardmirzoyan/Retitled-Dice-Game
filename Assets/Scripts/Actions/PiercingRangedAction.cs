using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Piercing Ranged")]
public class PiercingRangedAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // For each cardinal direction


        foreach (var location in room.GetNeighbors(startLocation, true))
        {
            int range = die.value;
            Vector3Int start = startLocation;
            Vector3Int direction = location - startLocation;
            // result.Add(room.GetFirstValidLocationWithinRange(startLocation, direction, die.value));

            while (room.IsValidLocation(start + direction, true, true, true) && range > 0)
            {
                start += direction;
                range--;
            }

            result.Add(start);
        }

        return result;
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
            if (!entity.room.IsValidLocation(start, true, true, true))
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
