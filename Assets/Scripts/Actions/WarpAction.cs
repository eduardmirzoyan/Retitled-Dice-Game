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
            if (!room.IsObsacle(location) && !room.HasEntity(location))
            {
                targets.Add(location);
            }
        }

        return targets;
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room)
    {
        return new List<Vector3Int>() { targetLocation };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // Handle visuals
        yield return entity.model.Warp(entity.location, targetLocation);

        // Relocate
        entity.Relocate(targetLocation);
    }
}
