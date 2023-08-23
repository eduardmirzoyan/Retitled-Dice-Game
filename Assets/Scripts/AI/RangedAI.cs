using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Ranged")]
public class RangedAI : AI
{
    /*
    Overview: Tries to get into "targetDistance" from the player while keeping line of sight, aims towards the player
    */

    [SerializeField] private float targetDistance = 6;

    public override List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity)
    {
        var actionPairSquence = new List<(Action, Vector3Int)>();

        // Get all possible actions
        var possibleActions = entity.GetActions();

        // ASSUME FIRST ACTION IS A MOVE ACTION
        var moveAction = possibleActions[0];

        // Choose the location that places you closest to target
        // NOTE: (0, 0, -1) means action will be ignored
        var moveLocation = GetOptimalLocationToTarget(entity.location, moveAction.GetValidLocations(entity.location, room), targetEntity.location, room);

        // Add pair
        actionPairSquence.Add((moveAction, moveLocation));

        // ASSUME SECOND ACTION IS A RANGED ACTION
        var rangedAction = possibleActions[1];

        // Set location
        if (moveLocation == Vector3Int.back)
            moveLocation = entity.location;

        // Choose to attack the tile that is closest to target (searching from new location AFTER moving)
        var rangedLocation = GetClosestLocationToTarget(entity.location, rangedAction.GetValidLocations(moveLocation, room), targetEntity.location);

        // Add pair
        actionPairSquence.Add((rangedAction, rangedLocation));

        return actionPairSquence;
    }

    private Vector3Int GetOptimalLocationToTarget(Vector3Int currentLocation, List<Vector3Int> locations, Vector3Int targetLocation, Room room)
    {
        // Try to get "targetDistance" from player while having line of sight

        float closest = Mathf.Abs(Vector3Int.Distance(currentLocation, targetLocation) - targetDistance);
        Vector3Int result = Vector3Int.back;

        foreach (var location in locations)
        {
            Debug.Log("Want to go from: " + location + " to " + targetLocation);

            // Make sure you have line of sight
            if (!room.IsValidPath(location, targetLocation, false, false))
                continue;

            var difference = Mathf.Abs(Vector3Int.Distance(location, targetLocation) - targetDistance);
            if (difference < closest)
            {
                closest = difference;
                result = location;
            }
        }

        // If we find a valid location, return it
        if (result != Vector3Int.back)
            return result;

        closest = Mathf.Abs(Vector3Int.Distance(currentLocation, targetLocation) - targetDistance);
        // Else just give up on line of sight and find optimal distance
        foreach (var location in locations)
        {
            var difference = Mathf.Abs(Vector3Int.Distance(location, targetLocation) - targetDistance);
            if (difference < closest)
            {
                closest = difference;
                result = location;
            }
        }

        return result;
    }

    private Vector3Int GetClosestLocationToTarget(Vector3Int currentLocation, List<Vector3Int> locations, Vector3Int targetLocation)
    {
        float closest = Vector3Int.Distance(currentLocation, targetLocation);
        Vector3Int result = Vector3Int.back;

        foreach (var location in locations)
        {
            var distance = Vector3Int.Distance(location, targetLocation);
            if (distance < closest)
            {
                closest = distance;
                result = location;
            }
        }

        return result;
    }
}
