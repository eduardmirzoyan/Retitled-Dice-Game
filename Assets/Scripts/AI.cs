using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AI : ScriptableObject
{
    // Hard coded so far, but action 0 should be move, action 1 is attack
    public (Action, Vector3Int) GenerateBestDecision(Entity entity, Room dungeon)
    {
        // Only consider the first action of the entity
        var moveAction = entity.actions[0];
        var meleeAction = entity.actions[1];

        // ~~~~ First check to see if player can be attacked ~~~

        // Loop through valid locations
        foreach (var location in meleeAction.GetValidLocations(entity.location, dungeon))
        {
            // If there is a location crosses the player
            if (IsBetween(entity.location, location, dungeon.player.location))
            {
                // Debug
                Debug.Log("Yielded true with the vectors, A: " + entity.location + " B: " + location + " C: " + dungeon.player.location);
                
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

    private bool IsBetween(Vector3Int point1, Vector3Int point2, Vector3Int currPoint) {
        int dxc = currPoint.x - point1.x;
        int dyc = currPoint.y - point1.y;

        int dxl = point2.x - point1.x;
        int dyl = point2.y - point1.y;

        int cross = dxc * dyl - dyc * dxl;

        if (cross != 0)
            return false;

        if (Mathf.Abs(dxl) >= Mathf.Abs(dyl))
            return dxl > 0 ?
              point1.x <= currPoint.x && currPoint.x <= point2.x :
              point2.x <= currPoint.x && currPoint.x <= point1.x;
        else
            return dyl > 0 ?
              point1.y <= currPoint.y && currPoint.y <= point2.y :
              point2.y <= currPoint.y && currPoint.y <= point1.y;
    }
}
