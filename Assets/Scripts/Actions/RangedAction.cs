using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Ranged")]
public class RangedAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        foreach (var location in room.GetNeighbors(startLocation, true))
        {
            int range = die.value;
            Vector3Int direction = location - startLocation;
            Vector3Int start = location;

            while (room.IsValidLocation(start, true, true, true) && range > 0)
            {
                if (room.IsEntity(start))
                {
                    result.Add(start);
                    start = startLocation;
                    break;
                }

                start += direction;
                range--;
            }

            if (start != startLocation && room.IsValidLocation(start - direction, true))
                result.Add(start - direction);
        }

        return result;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        return new List<Vector3Int>() { targetLocation };
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
