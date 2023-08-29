using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Jump")]
public class JumpAction : Action
{
    [SerializeField] private int radius = 5;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        for (int i = startLocation.x - radius; i <= startLocation.x + radius; i++)
        {
            for (int j = startLocation.y - radius; j <= startLocation.y + radius; j++)
            {
                var location = new Vector3Int(i, j, 0);
                if (room.IsValidLocation(location))
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
        yield return entity.Jump(targetLocation);
    }
}
