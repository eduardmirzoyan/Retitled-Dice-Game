using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Boss/Smash")]
public class SmashAction : Action
{
    [SerializeField] private int range = 6;

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
        Vector3Int left = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.forward));
        Vector3Int right = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.back));

        Vector3Int startMiddle = targetLocation;
        Vector3Int startLeft = targetLocation + left;
        Vector3Int startRight = targetLocation + right;

        // targets.Add(startMiddle);
        // targets.Add(startLeft);
        // targets.Add(startRight);

        for (int i = 0; i < range; i++)
        {
            // Add threats
            targets.Add(startMiddle);
            targets.Add(startLeft);
            targets.Add(startRight);

            // Increment
            startMiddle += forward;
            startLeft += forward;
            startRight += forward;
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
                entity.AttackEntity(target, weapon, GetTotalDamage());
            }
        }

        // Trigger event
        GameEvents.instance.TriggerOnEntityUseWeapon(entity, weapon);

        yield return null;
    }
}
