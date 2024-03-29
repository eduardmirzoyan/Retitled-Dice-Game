using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Jump")]
public class JumpAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        // Search in each direction
        foreach (var direction in cardinalDirections)
        {
            var location = startLocation + direction;
            var range = die.value;

            // Check range
            while (range > 0)
            {
                if (room.IsOutOfBounds(location) || room.IsWall(location))
                    break;

                if (!room.IsChasam(location) && !room.HasEntity(location))
                    targets.Add(location);

                location += direction;
                range--;
            }
        }

        return targets;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room)
    {
        return new List<Vector3Int>() { targetLocation };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // Wait for animation
        yield return entity.model.Jump(entity.location, targetLocation);

        // Relocate
        entity.Relocate(targetLocation);
    }
}
