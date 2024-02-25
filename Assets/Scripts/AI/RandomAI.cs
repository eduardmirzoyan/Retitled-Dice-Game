using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Random")]
public class RandomAI : AI
{
    /*
    Overview: Moves randomly, targets randomly
    */
    public override List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity)
    {
        var actionPairSquence = new List<(Action, Vector3Int)>();
        var possibleActions = entity.GetActions();
        if (possibleActions.Count < 2) throw new System.Exception("Not enough actions.");

        Vector3Int currentPosition = entity.location;

        // ASSUME FIRST ACTION IS A MOVE ACTION
        var moveAction = possibleActions[0];
        if (moveAction is not FreeMoveAction) throw new System.Exception("Action in first index must be a free move.");
        var validLocations = moveAction.GetValidLocations(entity.location, room);
        if (validLocations.Count > 0)
        {
            var moveLocation = validLocations[Random.Range(0, validLocations.Count)];
            actionPairSquence.Add((moveAction, moveLocation));

            // Update position after this action
            currentPosition = moveLocation;
        }

        // ASSUME SECOND ACTION IS AN ATTACK ACTION
        var attackAction = possibleActions[1];
        validLocations = attackAction.GetValidLocations(currentPosition, room);
        validLocations.Remove(currentPosition);
        if (validLocations.Count > 0)
        {
            var attackLocation = validLocations[Random.Range(0, validLocations.Count)];
            actionPairSquence.Add((attackAction, attackLocation));
        }

        return actionPairSquence;
    }
}
