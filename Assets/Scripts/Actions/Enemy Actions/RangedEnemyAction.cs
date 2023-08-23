using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Enemy/Ranged")]
public class RangedEnemyAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // For each cardinal direction
        foreach (var location in room.GetNeighbors(startLocation, true))
        {
            Vector3Int direction = location - startLocation;
            result.Add(room.GetFirstValidLocation(startLocation, direction, true));
        }

        return result;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Perform(Entity entity, List<Vector3Int> targetLocations, Room room)
    {
        // Attack location
        // entity.MeleeAttackLocation(targetLocation, weapon);

        yield return null;
    }

}
