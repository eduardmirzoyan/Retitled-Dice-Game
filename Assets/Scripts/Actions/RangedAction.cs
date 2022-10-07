using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RangedAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // Scale distanc based on die.value
        int reach = die.value;

        // Check in 4 cardinal direction based on die value

        // North
        Vector3Int endLocation = startLocation + new Vector3Int(0, reach, 0);
        if (room.IsValidPath(startLocation, endLocation, true, false))
        {
            result.Add(startLocation + new Vector3Int(0, reach, 0));
        }

        // South
        endLocation = startLocation + new Vector3Int(0, -reach, 0);
        if (room.IsValidPath(startLocation, endLocation, true, false))
        {
            result.Add(startLocation + new Vector3Int(0, -reach, 0));
        }

        // East
        endLocation = startLocation + new Vector3Int(reach, 0, 0);
        if (room.IsValidPath(startLocation, endLocation, true, false))
        {
            result.Add(startLocation + new Vector3Int(reach, 0, 0));
        }

        // West
        endLocation = startLocation + new Vector3Int(-reach, 0, 0);
        if (room.IsValidPath(startLocation, endLocation, true, false))
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

        // Wait for weapon to be drawn
        yield return new WaitForSeconds(Projectile.drawTime);

        // Spawn thrown weapon
        GameEvents.instance.TriggerOnEntityRangedAttack(entity, targetLocation, weapon);

        // Sheathe weapon
        GameEvents.instance.TriggerOnEntitySheatheWeapon(entity, weapon);
        
        // Wait for weapon to land
        yield return new WaitForSeconds(Projectile.travelSpeed);

        // Damage location
        entity.AttackLocation(targetLocation, weapon);

        // Finnish!
    }
}
