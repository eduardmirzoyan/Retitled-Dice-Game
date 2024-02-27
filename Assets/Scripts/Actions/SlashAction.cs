using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Slash")]
public class SlashAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        // Search in each direction
        foreach (var direction in cardinalDirections)
        {
            var location = startLocation + direction;
            var range = die.value;

            while (range > 0)
            {
                // Stop on obstacle
                if (room.IsWall(location) || room.IsChasam(location) || room.HasEntity(location))
                    break;

                targets.Add(location);

                location += direction;
                range--;
            }
        }

        return targets;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        // Threaten behind self
        Vector3Int direction = targetLocation - entity.location;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        return new List<Vector3Int>() { entity.location - direction };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // ~~~ Attack targets ~~~

        // Attack each location
        foreach (var location in threatenedLocations)
            entity.AttackLocation(location, weapon, GetTotalDamage());

        // Trigger event
        GameEvents.instance.TriggerOnEntityUseWeapon(entity, weapon);
        yield return null;

        // ~~~ Move to location ~~~

        // Calculate direction
        Vector3Int direction = targetLocation - entity.location;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        entity.model.MoveSetup();
        while (entity.location != targetLocation)
        {
            // Calculate next location
            Vector3Int nextLocation = entity.location + direction;

            // Run animations
            yield return entity.model.Move(entity.location, nextLocation);

            // Updatate data
            entity.Relocate(nextLocation);
        }
        entity.model.MoveCleanup();

    }
}
