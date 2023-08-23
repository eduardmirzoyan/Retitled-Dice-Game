using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Enemy/Melee")]
public class MeleeEnemyAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        return room.GetNeighbors(startLocation, true);
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
