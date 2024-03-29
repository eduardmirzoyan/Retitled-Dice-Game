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
                if (room.IsObsacle(location) || room.HasEntity(location))
                    break;

                targets.Add(location);

                location += direction;
                range--;
            }
        }

        return targets;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room)
    {
        // Threaten behind self
        Vector3Int direction = targetLocation - entity.location;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);
        Vector3Int location = entity.location - direction;

        if (room.IsObsacle(location))
            return new List<Vector3Int>();

        return new List<Vector3Int>() { location };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // Calculate direction
        Vector3Int direction = targetLocation - entity.location;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        // ~~~ Attack targets ~~~

        // Face backwards
        entity.model.FaceDirection(-direction);
        yield return weapon.model.Draw(-direction);

        // Attack each location
        foreach (var location in threatenedLocations)
            entity.AttackLocation(location, weapon, GetTotalDamage());

        // Handle visuals
        yield return weapon.model.Attack();
        yield return weapon.model.Sheathe();

        // ~~~ Move to location ~~~

        entity.model.MoveSetup();
        while (entity.location != targetLocation)
        {
            // Calculate next location
            Vector3Int nextLocation = entity.location + direction;

            // Handle visuals
            yield return entity.model.Move(entity.location, nextLocation);

            // Updatate data
            entity.Relocate(nextLocation);
        }
        entity.model.MoveCleanup();
    }
}
