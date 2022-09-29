using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RangedAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Dungeon dungeon)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Dungeon dungeon)
    {
        throw new System.NotImplementedException();
    }
}
