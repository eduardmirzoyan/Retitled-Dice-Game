using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Melee")]
public class MeleeAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // Scale distanc based on die.value
        int reach = die.value;

        // Check in 4 cardinal direction based on die value

        // North
        Vector3Int endLocation = startLocation + new Vector3Int(0, reach, 0);
        if (room.IsValidPath(startLocation, endLocation, true))
        {
            result.Add(startLocation + new Vector3Int(0, reach, 0));
        }

        // South
        endLocation = startLocation + new Vector3Int(0, -reach, 0);
        if (room.IsValidPath(startLocation, endLocation, true))
        {
            result.Add(startLocation + new Vector3Int(0, -reach, 0));
        }

        // East
        endLocation = startLocation + new Vector3Int(reach, 0, 0);
        if (room.IsValidPath(startLocation, endLocation, true))
        {
            result.Add(startLocation + new Vector3Int(reach, 0, 0));
        }

        // West
        endLocation = startLocation + new Vector3Int(-reach, 0, 0);
        if (room.IsValidPath(startLocation, endLocation, true))
        {
            result.Add(startLocation + new Vector3Int(-reach, 0, 0));
        }

        return result;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Room room)
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

        // Draw weapon
        GameEvents.instance.TriggerOnEntityDrawWeapon(entity, direction, weapon);

        // Trigger start move event
        GameEvents.instance.TriggerOnEntityStartMove(entity);

        // Keep looping until entity makes it to its final location
        while (entity.location != targetLocation)
        {
            // Move entity
            entity.MoveToward(direction);

            // Attack the location that you're at
            bool res = entity.MeleeAttackLocation(entity.location, weapon);

            // Trigger event
            if (res)
                GameEvents.instance.TriggerOnEntityMeleeAttack(entity, weapon);

            // Wait for animation
            yield return new WaitForSeconds(EntityModel.moveSpeed);
        }

        // Trigger stop move event
        GameEvents.instance.TriggerOnEntityStopMove(entity);

        // Sheathe weapon
        GameEvents.instance.TriggerOnEntitySheatheWeapon(entity, weapon);

        // Finnish!
    }
}
