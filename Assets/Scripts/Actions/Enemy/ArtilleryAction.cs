using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Artillery")]
public class ArtilleryAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        return new List<Vector3Int>() { room.player.location };
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>() { targetLocation };

        // Form + pattern
        foreach (var direction in cardinalDirections)
        {
            Vector3Int location = targetLocation + direction;

            if (room.IsObsacle(location))
                continue;

            targets.Add(location);
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
