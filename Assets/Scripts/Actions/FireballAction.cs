using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Fireball")]
public class FireballAction : Action
{
    [SerializeField] private bool useCrossPattern;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        // Check in each direction
        foreach (var direction in cardinalDirections)
        {
            var location = startLocation + direction * die.value;

            if (!room.IsWall(location))
            {
                targets.Add(location);
            }
        }

        return targets;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        List<Vector3Int> targets = new List<Vector3Int>() { targetLocation };

        if (useCrossPattern)
        {
            // Form + pattern
            foreach (var direction in cardinalDirections)
            {
                targets.Add(targetLocation + direction);
            }
        }
        else
        {
            // Form X pattern
            targets.Add(targetLocation + new Vector3Int(1, 1));
            targets.Add(targetLocation + new Vector3Int(1, -1));
            targets.Add(targetLocation + new Vector3Int(-1, 1));
            targets.Add(targetLocation + new Vector3Int(-1, -1));
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
