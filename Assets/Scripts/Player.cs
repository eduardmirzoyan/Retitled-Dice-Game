using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Player : Entity
{
    protected override void Interact()
    {
        int pickUpIndex = room.pickups[location.x][location.y];

        // Pick up any loot/coins
        switch (pickUpIndex)
        {
            case 1:
                // Use key
                room.UseKey();
                break;
            case 2:
                // Increment gold
                AddGold(1);
                break;
            default:
                // Nothing happened
                break;
        }

        // Trigger event
        GameEvents.instance.TriggerOnPickup(this, pickUpIndex);

        // Set any pickup to 0
        room.pickups[location.x][location.y] = 0;

        // If you are on an exit
        if (location == room.roomExit.location && !room.roomExit.IsLocked())
        {
            // Go to next floor
            GameManager.instance.TravelToNextFloor();
            
        }
    }

    protected override void OnDeath()
    {
        // Does nothing
    }
}
