using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Cleave")]
public class CleaveAction : Action
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
        // Direction towards target
        Vector3Int direction = targetLocation - entity.location;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        // Rotate right degree vector
        Vector3Int rotateRight = new(0, 0, 1);
        Vector3Int rotateLeft = new(0, 0, -1);

        Vector3Int leftLocation = targetLocation + Vector3Int.RoundToInt(Vector3.Cross(direction, rotateLeft));
        Vector3Int rightLocation = targetLocation + Vector3Int.RoundToInt(Vector3.Cross(direction, rotateRight));

        var list = new List<Vector3Int>();

        if (!room.IsObsacle(leftLocation))
            list.Add(leftLocation);

        if (!room.IsObsacle(rightLocation))
            list.Add(rightLocation);

        return list;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
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

        // ~~~ Attack targets ~~~

        entity.model.FaceDirection(direction);
        yield return weapon.model.Draw(direction);

        // Attack each location
        foreach (var location in threatenedLocations)
            entity.AttackLocation(location, weapon, GetTotalDamage());

        yield return weapon.model.Attack();
        yield return weapon.model.Sheathe();
    }
}
