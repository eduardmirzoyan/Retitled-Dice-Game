using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Player : Entity
{
    public override void MoveToward(Vector3Int direction)
    {
        // Make sure you are only moving 1 tile at a time
        if (direction.magnitude > 1) throw new System.Exception("DIRECTION MAG is NOT 1");

        // Increment location
        location += direction;

        // ~~~~~ This is where interactions happen ~~~~~

        int pickUpIndex = dungeon.pickups[location.x][location.y];

        // Pick up any loot/coins
        switch (pickUpIndex)
        {
            case 1:
                // Use key
                dungeon.UseKey();
                break;
            case 2:
                // Increment gold
                gold++;
                break;
            default:
                // Nothing happened
                break;
        }

        // Trigger event
        GameEvents.instance.TriggerOnPickup(this, pickUpIndex);

        // Set any pickup to 0
        dungeon.pickups[location.x][location.y] = 0;

        // Take damage from any enemies?

        // If you are on the exit
        if (dungeon.exitLocation == location && dungeon.ExitUnlocked())
        {
            // Go to next floor
            GameManager.instance.TravelToNextFloor();
        }
    }
}
