using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Defend")]
public class DefendAI : AI
{
    [SerializeField] private PickUpType pickupTypeToDefend = PickUpType.Key;

    /*
    Overview: Beelines to the nearest pickup. Once reached, continously attacks around it.
    */
    public override List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity)
    {
        // NOTE: (0, 0, -1) means action will be ignored
        var actionPairSquence = new List<(Action, Vector3Int)>();
        var possibleActions = entity.GetActions();
        Vector3Int currentPosition = entity.location;

        // Find closest pickup
        var pickupLocation = FindClosestPickup(pickupTypeToDefend, entity.location, room);
        if (pickupLocation != Vector3Int.back) // If pickup was found
        {
            // Chase pickup
            // Debug.Log($"Chasing {pickupTypeToDefend}");

            // ASSUME FIRST ACTION IS A MOVE ACTION
            var moveAction = possibleActions[0];
            if (moveAction is not FreeMoveAction)
                throw new System.Exception("First action of defendAI was not a free-move action.");

            // Choose the location that places you closest to target
            var moveLocation = GetClosestLocationToTarget(entity.location, pickupLocation, moveAction.GetValidLocations(entity.location, room));
            if (moveLocation != Vector3Int.back)
            {
                actionPairSquence.Add((moveAction, moveLocation));
                currentPosition = moveLocation;
            }

            // Check if on nearest pickup
            if (currentPosition == pickupLocation)
            {
                // If so, defend it...

                // Assume second action is attack
                var attackAction = possibleActions[1];
                actionPairSquence.Add((attackAction, targetEntity.location));
            }
        }
        else
        {
            // Chase player
            // Debug.Log($"Chasing {targetEntity.name}");

            // ASSUME FIRST ACTION IS A MOVE ACTION
            var moveAction = possibleActions[0];
            if (moveAction is not FreeMoveAction)
                throw new System.Exception("First action of defendAI was not a free-move action.");

            // Choose the location that places you closest to target
            var moveLocation = GetClosestLocationToTarget(entity.location, targetEntity.location, moveAction.GetValidLocations(entity.location, room));
            if (moveLocation != Vector3Int.back)
            {
                actionPairSquence.Add((moveAction, moveLocation));
                currentPosition = moveLocation;
            }

            // Check if on nearest pickup
            if (Room.ManhattanDistance(currentPosition, targetEntity.location) <= 1)
            {
                // Assume second action is attack
                var attackAction = possibleActions[1];
                actionPairSquence.Add((attackAction, targetEntity.location));
            }

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
