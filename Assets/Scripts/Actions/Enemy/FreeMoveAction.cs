using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Actions/Free Move")]
public class FreeMoveAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        // Range based on die value
        int range = die.value;

        var validLocations = new List<Vector3Int>();
        var previousLocations = new List<Vector3Int>
        {
            startLocation
        };

        // Loop based on range
        int stepCount = 0;
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

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room)
    {
        return new List<Vector3Int> { targetLocation };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // Generate path
        var path = room.pathfinder.FindPath(entity.location, targetLocation, room);

        // Remove the start location
        path.RemoveAt(0);

        entity.model.MoveSetup();
        while (path.Count > 0)
        {
            // Calculate next direction
            Vector3Int direction = path[0] - entity.location;
            if (direction.sqrMagnitude > 1)
                throw new System.Exception($"Improper move direction: {direction}");

            // Calculate next location
            Vector3Int nextLocation = entity.location + direction;

            // Run animations
            yield return entity.model.Move(entity.location, nextLocation);

            // Updatate data
            entity.Relocate(nextLocation);

            // Pop
            path.RemoveAt(0);
        }
        entity.model.MoveCleanup();
    }
}
