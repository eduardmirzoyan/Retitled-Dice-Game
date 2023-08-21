using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Melee")]
public class MeleeAI : AI
{
    /*
    Overview: Tries to get close as close as possible to the player, aims towards the player
    */

    public override List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity)
    {
        var actionPairSquence = new List<(Action, Vector3Int)>();

        // Get all possible actions
        var possibleActions = entity.GetActions();

        // ASSUME FIRST ACTION IS A MOVE ACTION
        var moveAction = possibleActions[0];

        // Choose the location that places you closest to target
        // NOTE: (0, 0, -1) means action will be ignored
        var moveLocation = GetClosestLocationToTarget(moveAction.GetValidLocations(entity.location, room), targetEntity.location);

        // Add pair
        actionPairSquence.Add((moveAction, moveLocation));


        // ASSUME SECOND ACTION IS A MELEE ACTION
        var meleeAction = possibleActions[1];

        // Choose to attack the tile that is closest to target (searching from new location AFTER moving)
        var meleeLocation = GetClosestLocationToTarget(meleeAction.GetValidLocations(moveLocation, room), targetEntity.location);

        // Add pair
        actionPairSquence.Add((meleeAction, meleeLocation));

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
