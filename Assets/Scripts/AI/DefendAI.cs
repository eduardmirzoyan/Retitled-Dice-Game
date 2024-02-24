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
        // NOTE: (0, 0, -1) means action will be ignored
        var actionPairSquence = new List<(Action, Vector3Int)>();

        // Get all possible actions
        var possibleActions = entity.GetActions();

        // ASSUME FIRST ACTION IS A MOVE ACTION
        var moveAction = possibleActions[0];
        if (moveAction is not FreeMoveAction)
            throw new System.Exception("First action of defendAI was not a free-move action.");

        Vector3Int currentPos = entity.location;

        var targetLocation = FindClosestPickup(pickupTypeToDefend, entity.location, room);
        // If pickup of that type was found AND you are not already on top of it...
        if (targetLocation != Vector3Int.back && entity.location != targetLocation)
        {
            // Choose the location that places you closest to target
            var moveLocation = GetClosestLocationToTarget(entity.location, targetLocation, moveAction.GetValidLocations(entity.location, room));
            if (moveLocation == Vector3Int.back)
                throw new System.Exception("Valid path to target not found.");

            actionPairSquence.Add((moveAction, moveLocation));

            // Update location
            currentPos = moveLocation;
        }

        // Check if target near you...
        if (Room.ManhattanDistance(currentPos, targetEntity.location) <= 1)
        {
            // Assume second action is attack
            var attackAction = possibleActions[1];
            actionPairSquence.Add((attackAction, currentPos));
        }

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
                var distance = Room.ManhattanDistance(currentLocation, tile.location);
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
