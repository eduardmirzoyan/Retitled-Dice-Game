using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Move")]
public class MoveAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // Scale distanc based on die.value
        int range = die.value;

        // Check in 4 cardinal direction based on die value

        // North
        Vector3Int location = startLocation;
        int count = range;
        while (room.IsValidLocation(location + Vector3Int.up) && count > 0)
        {
            location += Vector3Int.up;
            count--;
        }
        if (location != startLocation)
        {
            result.Add(location);
        }

        // South
        location = startLocation;
        count = range;
        while (room.IsValidLocation(location + Vector3Int.down) && count > 0)
        {
            location += Vector3Int.down;
            count--;
        }
        if (location != startLocation)
        {
            result.Add(location);
        }

        // East
        location = startLocation;
        count = range;
        while (room.IsValidLocation(location + Vector3Int.right) && count > 0)
        {
            location += Vector3Int.right;
            count--;
        }
        if (location != startLocation)
        {
            result.Add(location);
        }

        // West
        location = startLocation;
        count = range;
        while (room.IsValidLocation(location + Vector3Int.left) && count > 0)
        {
            location += Vector3Int.left;
            count--;
        }
        if (location != startLocation)
        {
            result.Add(location);
        }

        return result;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        return new List<Vector3Int> { targetLocation };
    }


    public override IEnumerator Perform(Entity entity, List<Vector3Int> targetLocations, Room room)
    {
        var targetLocation = targetLocations[0];

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

        // Finnish!
    }
}
