using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Ranged")]
public class RangedAction : Action
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
                // If we hit a wall or OoB
                if (room.IsOutOfBounds(location) || room.IsWall(location))
                {
                    location -= direction;
                    if (location != startLocation)
                        targets.Add(location);

                    break;
                }

                // If we find a target
                if (room.HasEntity(location))
                {
                    targets.Add(location);
                    break;
                }

                // If we reach end of range
                if (range == 1)
                {
                    targets.Add(location);
                    break;
                }

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
        Vector3Int direction = targetLocation - entity.location;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        // Face forward
        entity.model.FaceDirection(direction);
        yield return weapon.model.Draw(direction);

        // Damage tile
        foreach (var location in threatenedLocations)
            entity.AttackLocation(location, weapon, GetTotalDamage());

        // Handle visuals
        yield return weapon.model.Attack();
        yield return weapon.model.Sheathe();
    }
}
