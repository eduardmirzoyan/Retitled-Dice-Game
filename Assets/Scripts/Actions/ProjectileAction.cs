using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Fire Projectile")]
public class ProjectileAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Room room)
    {
        // Spawn projectile

        yield return null;

        
    }
}
