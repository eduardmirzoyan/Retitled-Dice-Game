using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Melee")]
public class MeleeAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        foreach (var direction in cardinalDirections)
        {
            var location = startLocation + direction;

            if (!room.IsWall(location))
            {
                targets.Add(location);
            }
        }

        return targets;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        Vector3Int forward = targetLocation - entity.location;

        switch (die.value)
        {
            case 1:
                {
                    // Just one target
                    targets.Add(targetLocation);
                }
                break;
            case 2:
                {
                    Vector3Int left = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.forward));
                    Vector3Int right = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.back));

                    // 3 targets
                    targets.Add(targetLocation);
                    targets.Add(targetLocation + left);
                    targets.Add(targetLocation + right);
                }
                break;
            case 3:
                {
                    Vector3Int left = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.forward));
                    Vector3Int right = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.back));
                    Vector3Int leftDown = Vector3Int.RoundToInt(Vector3.Cross(left, Vector3Int.forward));
                    Vector3Int rightDown = Vector3Int.RoundToInt(Vector3.Cross(right, Vector3Int.back));

                    // 5 targets
                    targets.Add(targetLocation);
                    targets.Add(targetLocation + left);
                    targets.Add(targetLocation + right);
                    targets.Add(targetLocation + left + leftDown);
                    targets.Add(targetLocation + right + rightDown);
                }
                break;
            case 4:
                {
                    Vector3Int left = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.forward));
                    Vector3Int right = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.back));
                    Vector3Int leftDown = Vector3Int.RoundToInt(Vector3.Cross(left, Vector3Int.forward));
                    Vector3Int rightDown = Vector3Int.RoundToInt(Vector3.Cross(right, Vector3Int.back));

                    // 7 targets
                    targets.Add(targetLocation);
                    targets.Add(targetLocation + left);
                    targets.Add(targetLocation + right);
                    targets.Add(targetLocation + left + leftDown);
                    targets.Add(targetLocation + right + rightDown);
                    targets.Add(targetLocation + left + leftDown + leftDown);
                    targets.Add(targetLocation + right + rightDown + rightDown);
                }
                break;
            default:
                throw new System.Exception("CANNOT HANDLE DIE VALUE: " + die.value);

        }


        return targets;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        foreach (var location in threatenedLocations)
        {
            var target = room.GetEntityAtLocation(location);
            if (target != null)
            {
                entity.Attack(target);
            }
        }

        // Trigger event
        GameEvents.instance.TriggerOnEntityUseWeapon(entity, weapon);

        // Wait for animation
        yield return null;
    }
}
