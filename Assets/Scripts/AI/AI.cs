using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AI : ScriptableObject
{
    public bool isHostile = true;

    public virtual void DisplayIntent(Entity entity, Room room)
    {
        // Does nothing
    }
    
    // Hard coded so far, but action 0 should be move, action 1 is attack
    public virtual (Action, Vector3Int) GenerateBestDecision(Entity entity, Room dungeon)
    {
        // Only consider the first action of the entity
        var moveAction = entity.GetActions()[0];
        var meleeAction = entity.GetActions()[1]; 

        // ~~~~ First check to see if player can be attacked ~~~

        // Loop through valid locations
        foreach (var location in meleeAction.GetValidLocations(entity.location, dungeon))
        {
            // If there is a location crosses the player
            if (IsBetween(entity.location, location, dungeon.player.location))
            {
                // Debug
                // Debug.Log("Yielded true with the vectors, A: " + entity.location + " B: " + location + " C: " + dungeon.player.location);
                
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
