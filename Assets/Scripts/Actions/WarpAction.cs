using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WarpAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // Scale distanc based on die.value
        int reach = die.value;

        // Check the ENDPOINTS ONLY in the 4 cardinal directions based on die value

        // North
        Vector3Int endLocation = startLocation + new Vector3Int(0, reach, 0);
        if (room.IsValidLocation(endLocation))
        {
            result.Add(startLocation + new Vector3Int(0, reach, 0));
        }

        // South
        endLocation = startLocation + new Vector3Int(0, -reach, 0);
        if (room.IsValidLocation(endLocation))
        {
            result.Add(startLocation + new Vector3Int(0, -reach, 0));
        }

        // East
        endLocation = startLocation + new Vector3Int(reach, 0, 0);
        if (room.IsValidLocation(endLocation))
        {
            result.Add(startLocation + new Vector3Int(reach, 0, 0));
        }

        // West
        endLocation = startLocation + new Vector3Int(-reach, 0, 0);
        if (room.IsValidLocation(endLocation))
        {
            result.Add(startLocation + new Vector3Int(-reach, 0, 0));
        }

        return result;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, Room room)
    {
        // Warp to location
        entity.WarpTo(targetLocation);

        // Wait for animation
        yield return new WaitForSeconds(EntityModel.warpSpeed);

        // Trigger event
        GameEvents.instance.TriggerOnEnityWarp(entity);

        // Finnish!
    }
}
