using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Warp")]
public class WarpAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        // Search in each direction
        foreach (var direction in cardinalDirections)
        {
            var location = startLocation + (direction * die.value);
            if (!room.IsWall(location) && !room.IsChasam(location) && !room.HasEntity(location))
            {
                targets.Add(location);
            }
        }

        return targets;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        return new List<Vector3Int>() { targetLocation };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // Warp to location
        // yield return entity.WarpTo(targetLocation);

        yield return entity.model.Warp(entity.location, targetLocation); // new WaitForSeconds(GameManager.instance.gameSettings.jumpBufferTime);

        // Relocate
        entity.Relocate(targetLocation);
    }
}
