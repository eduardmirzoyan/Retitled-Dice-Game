using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Actions/Free Move")]
public class FreeMoveAction : Action
{
    // FINISH

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        // Range based on die value
        int range = die.value;

        var validLocations = new List<Vector3Int>();
        int stepCount = 0;

        // Store previous locations
        var previousLocations = new List<Vector3Int>();
        previousLocations.Add(startLocation);

        // Loop based on range
        while (stepCount < range)
        {
            var surrounding = new List<Vector3Int>();

            foreach (var location in previousLocations)
            {
                // Get all neighbors
                surrounding.AddRange(room.GetNeighbors(location));
            }

            // Add all those
            validLocations.AddRange(surrounding);

            // Update previous locations
            previousLocations = surrounding.Distinct().ToList();

            // Increment step
            stepCount++;
        }

        return validLocations.Distinct().ToList();
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        return new List<Vector3Int> { targetLocation };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Room room)
    {
        // Generate path
        var path = room.pathfinder.FindPath(entity.location, targetLocation, room);

        // Remove the start location
        path.RemoveAt(0);

        // Trigger start move event
        GameEvents.instance.TriggerOnEntityStartMove(entity);

        // Keep looping while there is a path
        while (path.Count > 0)
        {
            // Calculate direction
            Vector3Int direction = path[0] - entity.location;

            // Move entity
            yield return entity.MoveToward(direction);

            // Pop
            path.RemoveAt(0);
        }

        // Trigger stop move event
        GameEvents.instance.TriggerOnEntityStopMove(entity);

        // Do nothing for now
        yield return null;
    }
}
