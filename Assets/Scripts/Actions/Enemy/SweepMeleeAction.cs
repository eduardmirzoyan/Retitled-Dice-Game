using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Enemy/Sweep Melee")]
public class SweepMeleeAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        foreach (var direction in cardinalDirections)
        {
            var location = startLocation + direction;

            if (room.IsObsacle(location))
                continue;

            targets.Add(startLocation + direction);
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

        var list = new List<Vector3Int>() { targetLocation };

        if (!room.IsObsacle(leftLocation))
            list.Add(leftLocation);

        if (!room.IsObsacle(rightLocation))
            list.Add(rightLocation);

        return list;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        foreach (var location in threatenedLocations)
            entity.AttackLocation(location, weapon, GetTotalDamage());

        yield return weapon.model.Attack();
        yield return weapon.model.Sheathe();
    }
}
