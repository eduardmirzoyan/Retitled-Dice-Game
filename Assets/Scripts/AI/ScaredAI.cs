using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves as far from the player as possible. 
/// Attacks as near at the player as possible
/// </summary>
[CreateAssetMenu(menuName = "AI/Scared")]
public class ScaredAI : AI
{
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
        var moveLocation = GetFarthestLocationToTarget(entity.location, targetEntity.location, moveAction.GetValidLocations(entity.location, room));

        // Add pair
        actionPairSquence.Add((moveAction, moveLocation));

        // Reset location if needed
        if (moveLocation == Vector3Int.back)
            moveLocation = entity.location;

        // ASSUME SECOND ACTION IS AN ATTACK ACTION
        var attackAction = possibleActions[1];

        // Choose to attack the tile that is closest to target (searching from new location AFTER moving)
        var attackLocation = GetClosestLocationToTarget(moveLocation, targetEntity.location, attackAction.GetValidLocations(moveLocation, room));

        // Add pair
        actionPairSquence.Add((attackAction, attackLocation));

        return actionPairSquence;
    }
}
