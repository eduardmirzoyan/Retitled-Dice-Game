using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AI : ScriptableObject
{
    public (Action, Vector3Int) GenerateBestDecision(Entity entity, Dungeon dungeon) {
        // Only consider the first action of the entity
        var action = entity.actions[0];

        float closestDistance = float.MaxValue;
        Vector3Int bestLocation = Vector3Int.zero;

        // Choose the location that is closest to the player

        // Loop through valid locations
        foreach (var location in action.GetValidLocations(entity.location, dungeon)) {
            // Calculate distance from player
            float distanceFromPlayer = Vector3.Distance(dungeon.player.location, location);
            if (distanceFromPlayer < closestDistance) {
                closestDistance = distanceFromPlayer;
                bestLocation = location;
            }
        }

        // Return the choice pair
        return (action, bestLocation);
    }
}
