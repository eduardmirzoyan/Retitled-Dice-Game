using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Artillery")]
public class ArtilleryAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        return new List<Vector3Int>() { room.player.location };
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        List<Vector3Int> targets = new List<Vector3Int>() { targetLocation };

        // Form + pattern
        foreach (var direction in cardinalDirections)
        {
            targets.Add(targetLocation + direction);
        }

        return targets;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // Logic
        foreach (var location in threatenedLocations)
        {
            // Damage first target found
            var target = room.GetEntityAtLocation(location);
            if (target != null)
            {
                // Damage location
                entity.AttackEntity(target, weapon, GetTotalDamage());

                // Dip
                break;
            }
        }

        // Trigger event
        GameEvents.instance.TriggerOnEntityUseWeapon(entity, weapon);

        yield return null;
    }
}
