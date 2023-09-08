using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Fireball")]
public class FireballAction : Action
{
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

        // Add each direction
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
                entity.Attack(target, weapon);

                // Dip
                break;
            }
        }

        // Trigger event
        GameEvents.instance.TriggerOnEntityUseWeapon(entity, weapon);

        yield return null;
    }
}
