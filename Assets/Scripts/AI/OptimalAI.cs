using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Optimal")]
public class OptimalAI : AI
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
        if (moveAction is not FreeMoveAction)
            throw new System.Exception("Action in first index must be a free move.");

        // Choose the location that places you closest to target
        // NOTE: (0, 0, -1) means action will be ignored
        var moveLocation = GetOptimalLocationToTarget(entity.location, moveAction.GetValidLocations(entity.location, room), targetEntity.location, room);

        // Add pair
        actionPairSquence.Add((moveAction, moveLocation));

        // Reset location if needed
        if (moveLocation == Vector3Int.back)
            moveLocation = entity.location;

        // ASSUME SECOND ACTION IS A RANGED ACTION
        var attackAction = possibleActions[1];

        // Choose to attack the tile that is closest to target (searching from new location AFTER moving)
        var attackLocation = GetLocationInDirectionOfTarget(entity.location, attackAction.GetValidLocations(moveLocation, room), targetEntity.location);

        // Add pair
        actionPairSquence.Add((attackAction, attackLocation));

        return actionPairSquence;
    }

    private Vector3Int GetOptimalLocationToTarget(Vector3Int currentLocation, List<Vector3Int> locations, Vector3Int targetLocation, Room room)
    {
        // Try to get "targetDistance" from player while having line of sight

        float farthest = Vector3Int.Distance(currentLocation, targetLocation);
        Vector3Int result = Vector3Int.back;

        foreach (var location in locations)
        {
            var distance = Vector3Int.Distance(location, targetLocation);
            if (HasLineOfSight(location, targetLocation, room) && distance > farthest)
            {
                farthest = distance;
                result = location;
            }
        }

        // If we found a location within line of sight, return it
        if (result != Vector3Int.back)
            return result;

        // Else just give up on line of sight and find optimal distance
        var optimal = Mathf.Abs(Vector3Int.Distance(currentLocation, targetLocation) - targetDistance);
        foreach (var location in locations)
        {
            var difference = Mathf.Abs(Vector3Int.Distance(location, targetLocation) - targetDistance);
            if (difference < optimal)
            {
                optimal = difference;
                result = location;
            }
        }

        return result;
    }

    private Vector3Int GetLocationInDirectionOfTarget(Vector3Int currentLocation, List<Vector3Int> locations, Vector3Int targetLocation)
    {
        Vector3Int result = Vector3Int.back;
        Vector3Int targetDirection = targetLocation - currentLocation;

        float closest = float.MaxValue;
        foreach (var location in locations)
        {
            Vector3Int direction = location - currentLocation;
            var distance = Vector3Int.Distance(targetDirection, direction);
            if (distance < closest)
            {
                closest = distance;
                result = location;
            }
        }

        return result;
    }
}
