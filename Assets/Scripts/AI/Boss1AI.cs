using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Boss/1")]
public class Boss1AI : AI
{
    [SerializeField] private int meleeRange = 3;
    /*

    [0] - Movement
    [1] - Melee 1
    [2] - Melee 2
    [3] - Ranged 1
    [4] - Ranged 2
    
    */
    public override List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity)
    {
        var actionPairSquence = new List<(Action, Vector3Int)>();

        // Get all possible actions
        var possibleActions = entity.AllActions();

        // ASSUME FIRST ACTION IS A MOVE ACTION
        var moveAction = possibleActions[0];
        if (moveAction is not FreeMoveAction)
            throw new System.Exception("Action in first index must be a free move.");

        // Choose the location that places you closest to target
        // NOTE: (0, 0, -1) means action will be ignored
        var moveLocation = GetClosestLocationToTarget(moveAction.GetValidLocations(entity.location, room), targetEntity.location);

        // Add pair
        actionPairSquence.Add((moveAction, moveLocation));
        if (moveLocation == Vector3Int.back)
            moveLocation = entity.location;

        // If far
        if (Vector3Int.Distance(targetEntity.location, moveLocation) > meleeRange)
        {
            // Use ranged attacks
            int choice = Random.Range(3, 5);
            var rangedAction = possibleActions[choice];
            var locations = rangedAction.GetValidLocations(moveLocation, room);

            // Add pair
            actionPairSquence.Add((rangedAction, locations[0]));
        }
        // If close
        else
        {
            // Use melee attacks
            int choice = Random.Range(1, 3);
            var meleeAction = possibleActions[choice];
            var meleeLocation = GetClosestLocationToTarget(meleeAction.GetValidLocations(moveLocation, room), targetEntity.location);

            // Add pair
            actionPairSquence.Add((meleeAction, meleeLocation));
        }

        return actionPairSquence;
    }

    private Vector3Int GetClosestLocationToTarget(List<Vector3Int> locations, Vector3Int targetLocation)
    {
        float closest = float.MaxValue;
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
