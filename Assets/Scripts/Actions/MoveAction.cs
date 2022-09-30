using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoveAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Dungeon dungeon)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // Scale distanc based on die.value
        int reach = die.value;

        // Check in 4 cardinal direction based on die value

        // North
        Vector3Int endLocation = startLocation + new Vector3Int(0, reach, 0);
        if (dungeon.IsValidPath(startLocation, endLocation))
        {
            result.Add(startLocation + new Vector3Int(0, reach, 0));
        }

        // South
        endLocation = startLocation + new Vector3Int(0, -reach, 0);
        if (dungeon.IsValidPath(startLocation, endLocation))
        {
            result.Add(startLocation + new Vector3Int(0, -reach, 0));
        }

        // East
        endLocation = startLocation + new Vector3Int(reach, 0, 0);
        if (dungeon.IsValidPath(startLocation, endLocation))
        {
            result.Add(startLocation + new Vector3Int(reach, 0, 0));
        }

        // West
        endLocation = startLocation + new Vector3Int(-reach, 0, 0);
        if (dungeon.IsValidPath(startLocation, endLocation))
        {
            result.Add(startLocation + new Vector3Int(-reach, 0, 0));
        }

        return result;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Dungeon dungeon)
    {
        // Calculate direction
        Vector3Int direction = targetLocation - entity.location;

        if (direction.x > 0) // Move right
        {
            direction.x = 1;
        }
        else if (direction.x < 0) // Move left
        { 
            direction.x = -1;
        }
        else if (direction.y > 0) // Move up
        {
            direction.y = 1;
        }
        else if (direction.y < 0) // Move down
        {
            direction.y = -1;
        }
        else
        {
            // Debug
            throw new System.Exception("There was a problem with determining direction.");
        }

        // Keep looping until entiy makes it to its final location
        while (entity.location != targetLocation) {
            // Move entity
            entity.MoveToward(direction);

            // Trigger move event
            GameEvents.instance.TriggerOnEntityMove(entity, true);
            
            // Wait for animation
            yield return new WaitForSeconds(EntityModel.moveSpeed);
        }

        // Trigger stop event
        GameEvents.instance.TriggerOnEntityMove(entity, false);

        // Finnish!
    }
}
