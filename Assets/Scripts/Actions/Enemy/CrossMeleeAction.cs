using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Enemy/Cross Melee")]
public class CrossMeleeAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        return new List<Vector3Int>() { startLocation };
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        int range = die.value;

        // Search in each direction up to range
        foreach (var direction in cardinalDirections)
        {
            for (int i = 1; i < range + 1; i++)
            {
                var location = entity.location + direction * i;
                if (entity.room.IsWall(location))
                    break;

                targets.Add(location);
            }
        }

        return targets;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // ?

        foreach (var location in threatenedLocations)
            entity.AttackLocation(location, weapon, GetTotalDamage());

        yield return weapon.model.Attack();
        yield return weapon.model.Sheathe();
    }
}
