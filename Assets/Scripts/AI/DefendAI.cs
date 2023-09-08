using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Defend")]
public class DefendAI : AI
{
    [SerializeField] private PickUpType pickupTypeToDefend = PickUpType.Key;
    /*
    Overview: Tries to get close as close as possible to a pickup and stays there. Does not attack.
    */
    public override List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity)
    {
        var actionPairSquence = new List<(Action, Vector3Int)>();

        // Get all possible actions
        var possibleActions = entity.GetActions();

        // ASSUME FIRST ACTION IS A MOVE ACTION
        var moveAction = possibleActions[0];

        var targetLocation = FindClosestPickup(pickupTypeToDefend, entity.location, room);
        // If no pickup of that type was found, do nothing.
        if (targetLocation == Vector3Int.back)
            return actionPairSquence;

        // Choose the location that places you closest to target
        // NOTE: (0, 0, -1) means action will be ignored
        var moveLocation = GetClosestLocationToTarget(entity.location, targetLocation, moveAction.GetValidLocations(entity.location, room));

        // Add pair
        actionPairSquence.Add((moveAction, moveLocation));

        // Finish
        return actionPairSquence;
    }

    private Vector3Int FindClosestPickup(PickUpType pickUpType, Vector3Int currentLocation, Room room)
    {
        float closest = float.MaxValue;
        Vector3Int result = Vector3Int.back;

        foreach (var tile in room.tiles)
        {
            if (tile.containedPickup == pickUpType)
            {
                var distance = Vector3Int.Distance(currentLocation, tile.location);
                if (distance < closest)
                {
                    closest = distance;
                    result = tile.location;
                }
            }
        }

        return result;
    }
}
