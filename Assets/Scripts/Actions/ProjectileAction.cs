using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Fire Projectile")]
public class ProjectileAction : Action
{
    public GameObject projectilePrefab;
    public Entity projectile;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // Scale distanc based on die.value
        int maxRange = die.value;

        // Check in 4 cardinal direction based on die value

        // North
        Vector3Int endLocation = startLocation + new Vector3Int(0, maxRange, 0);
        Vector3Int position = startLocation;
        Vector3Int direction = Vector3Int.up;
        bool prematureFlag = false;

        // Loop through all tiles
        while (position != endLocation)
        {
            // Increment
            position += direction;

            // Make sure tile is valid
            if (!room.IsValidLocation(position, true)) 
            {
                if (position - direction != startLocation) 
                {
                    // Add previous location
                    result.Add(position - direction);
                    // Set flag
                    prematureFlag = true;
                    break;
                }
            }
        }
        if (!prematureFlag)
        {
            result.Add(endLocation);
        }
        

        // South
        endLocation = startLocation + new Vector3Int(0, -maxRange, 0);
        position = startLocation;
        direction = Vector3Int.down;
        prematureFlag = false;

        // Loop through all tiles
        while (position != endLocation)
        {
            // Increment
            position += direction;

            // Make sure tile is valid
            if (!room.IsValidLocation(position, true))
            {
                if (position - direction != startLocation)
                {
                    // Add previous location
                    result.Add(position - direction);
                    // Set flag
                    prematureFlag = true;
                    break;
                }
            }
        }
        if (!prematureFlag)
        {
            result.Add(endLocation);
        }

        // East
        endLocation = startLocation + new Vector3Int(maxRange, 0, 0);
        position = startLocation;
        direction = Vector3Int.right;
        prematureFlag = false;

        // Loop through all tiles
        while (position != endLocation)
        {
            // Increment
            position += direction;

            // Make sure tile is valid
            if (!room.IsValidLocation(position, true))
            {
                if (position - direction != startLocation)
                {
                    // Add previous location
                    result.Add(position - direction);
                    // Set flag
                    prematureFlag = true;
                    break;
                }
            }
        }
        if (!prematureFlag)
        {
            result.Add(endLocation);
        }

        // West
        endLocation = startLocation + new Vector3Int(-maxRange, 0, 0);
        position = startLocation;
        direction = Vector3Int.left;
        prematureFlag = false;

        // Loop through all tiles
        while (position != endLocation)
        {
            // Increment
            position += direction;

            // Make sure tile is valid
            if (!room.IsValidLocation(position, true))
            {
                if (position - direction != startLocation)
                {
                    // Add previous location
                    result.Add(position - direction);
                    // Set flag
                    prematureFlag = true;
                    break;
                }
            }
        }
        if (!prematureFlag)
        {
            result.Add(endLocation);
        }

        return result;
    }


    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Room room)
    {
        // Calculate direction
        Vector3Int direction = (targetLocation - entity.location);
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        // ~~~ Projectile logic ~~~

        // Make a copy
        var copy = projectile.Copy();
        copy.SetRoom(room, entity.location);

        // Spawn projectile
        GameEvents.instance.TriggerOnSpawnEnity(copy);

        // Start moving
        GameEvents.instance.TriggerOnEntityStartMove(copy, direction);

        // Keep looping until entiy makes it to its final location
        while (copy.location != targetLocation)
        {
            // Move entity
            copy.MoveToward(direction);

            // Make sure you don't attack the spawner
            if (copy.location != entity.location) {

                // Attack the location that you're at
                bool res = copy.AttackLocation(copy.location, weapon);

                // Trigger event
                if (res)
                {
                    // Finish
                    break;
                }
            }

            // Wait for animation
            yield return new WaitForSeconds(EntityModel.moveSpeed);
        }

        // Stop moving
        GameEvents.instance.TriggerOnEntityStopMove(copy);

        // Destroy proj
        GameEvents.instance.TriggerOnRemoveEnity(copy);
    }
}
