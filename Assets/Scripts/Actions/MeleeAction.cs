using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Melee")]
public class MeleeAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        return room.GetNeighbors(startLocation, true);
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        Vector3Int forward = targetLocation - entity.location;

        switch (die.value)
        {
            case 1:
            case 2:
                {
                    // Just one target
                    targets.Add(targetLocation);
                }
                break;
            case 3:
            case 4:
                {
                    Vector3Int left = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.forward));
                    Vector3Int right = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.back));

                    // 3 targets
                    targets.Add(targetLocation);
                    targets.Add(targetLocation + left);
                    targets.Add(targetLocation + right);
                }
                break;
            case 5:
            case 6:
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
            case 7:
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

    public override IEnumerator Perform(Entity entity, List<Vector3Int> targetLocations, Room room)
    {
        // Calculate direction
        //Vector3Int direction = targetLocation - entity.location;
        //direction.Clamp(-Vector3Int.one, Vector3Int.one);

        // Draw weapon
        GameEvents.instance.TriggerOnEntityDrawWeapon(entity, Vector3.left, weapon);

        // Wait for animation
        yield return new WaitForSeconds(EntityModel.moveSpeed);

        foreach (var location in targetLocations)
        {
            var target = room.GetEntityAtLocation(location);
            if (target != null)
            {
                entity.MeleeAttackEntity(target);

                // Wait for animation
                yield return new WaitForSeconds(EntityModel.moveSpeed);
            }
        }

        // Attack the location that you're at
        // entity.MeleeAttackLocation(targetLocation, weapon);

        // Sheathe weapon
        GameEvents.instance.TriggerOnEntitySheatheWeapon(entity, weapon);

        // Finnish!
    }
}
