using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Move")]
public class MoveAction : Action
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
                if (room.IsWall(location) || room.IsChasam(location) || room.HasEntity(location))
                {
                    break;
                }

                targets.Add(location);

                location += direction;
                range--;
            }
        }

        return targets;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        return new List<Vector3Int>() { targetLocation };
    }


    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // Calculate direction
        Vector3Int direction = targetLocation - entity.location;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        // Trigger start move event
        GameEvents.instance.TriggerOnEntityStartMove(entity);

        // Keep looping until entiy makes it to its final location
        while (entity.location != targetLocation)
        {
            // Move entity
            yield return entity.MoveToward(direction); // Convert this into IEnumator
        }

        // Trigger stop move event
        GameEvents.instance.TriggerOnEntityStopMove(entity);
    }
}
