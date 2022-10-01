using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AI : ScriptableObject
{
    // Hard coded so far, but action 0 should be move, action 1 is attack
    public (Action, Vector3Int) GenerateBestDecision(Entity entity, Dungeon dungeon)
    {
        // Only consider the first action of the entity
        var moveAction = entity.actions[0];
        var meleeAction = entity.actions[1];

        // ~~~~ First check to see if player can be attacked ~~~

        // Loop through valid locations
        foreach (var location in meleeAction.GetValidLocations(entity.location, dungeon))
        {
            // If there is a location crosses the player
            if (IsCBetweenAB(entity.location, location, dungeon.player.location))
            {
                // Finish
                return (meleeAction, location);
            }
        }


        // ~~~ Else go to location that is closest to the player ~~~
        
        // Store variables
        float closestDistance = Vector3.Distance(dungeon.player.location, entity.location);
        Vector3Int bestLocation = Vector3Int.back;
        
        // Loop through valid locations
        foreach (var location in moveAction.GetValidLocations(entity.location, dungeon))
        {
            // Calculate distance from player
            float distanceFromPlayer = Vector3.Distance(dungeon.player.location, location);
            if (distanceFromPlayer < closestDistance)
            {
                closestDistance = distanceFromPlayer;
                bestLocation = location;
            }
        }

        // Returns Vector.z = -1 if entity won't perform any action
        // Return the choice pair
        return (moveAction, bestLocation);
    }

    private bool IntersectsTarget(Vector3 source, Vector3 location, Vector3 target)
    {
        // To find if action goes through player, calculate 2 lines and take cross, if == 0, then true!
        Vector3 line1 = target - source;
        Vector3 line2 = target - location;
        Vector3.Cross(line1, line2);

        return false;
    }

    private bool IsCBetweenAB(Vector3 A, Vector3 B, Vector3 C)
    {
        return Vector3.Dot((B - A).normalized, (C - B).normalized) < 0f && Vector3.Dot((A - B).normalized, (C - A).normalized) < 0f;
    }
}
