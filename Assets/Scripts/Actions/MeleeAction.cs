using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MeleeAction : Action
{
    // TODO THIS, after enemy tho lol

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Dungeon dungeon)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Dungeon dungeon)
    {
        throw new System.NotImplementedException();
    }
}
