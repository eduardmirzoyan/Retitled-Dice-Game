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

            if (room.IsObsacle(location))
                continue;

            targets.Add(location);
        }

        return targets;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        Vector3Int forward = targetLocation - entity.location;
        Vector3Int left = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.forward));
        Vector3Int right = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.back));

        Vector3Int startMiddle = targetLocation;
        Vector3Int startLeft = targetLocation + left;
        Vector3Int startRight = targetLocation + right;

        for (int i = 0; i < range; i++)
        {
            // Add threats
            if (!room.IsObsacle(startMiddle))
                targets.Add(startMiddle);
            if (!room.IsObsacle(startLeft))
                targets.Add(startLeft);
            if (!room.IsObsacle(startRight))
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
            entity.AttackLocation(location, weapon, GetTotalDamage());

        yield return weapon.model.Attack();
        yield return weapon.model.Sheathe();
    }
}
