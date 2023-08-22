using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectiles/Basic")]
public class BasicProjectile : Projectil
{
    public override IEnumerator Travel(Vector3Int direction, int range)
    {
        // Start moving
        GameEvents.instance.TriggerOnProjectileMoveStart(this);

        // Keep looping until out of range
        while (range > 0)
        {
            // Move entity
            MoveToward(direction);

            // Make sure you don't attack the spawner
            if (location != entity.location)
            {

                // Attempt to attack location, if so break
                if (AttackLocation()) break;
            }

            // Trigger event
            // GameEvents.instance.TriggerOnProjectileMove(this);

            // Wait for animation
            yield return new WaitForSeconds(travelSpeed);

            // Decrement
            range--;
        }

        // Stop moving
        GameEvents.instance.TriggerOnProjectileMoveStop(this);
    }

    private bool AttackLocation()
    {
        var targets = new List<Entity>();

        // Consider player, enemies and barrels
        targets.Add(room.player);
        targets.AddRange(room.hostileEntities);
        targets.AddRange(room.neutralEntities);

        // Check if any entities are on the same tile, if so damage them
        foreach (var target in targets)
        {
            // If the target is on the same time
            if (location == target.location)
            {
                // Currently deal 1 damage, but this might change?
                target.TakeDamage(damage);

                return true;
            }
        }

        return false;
    }

}
