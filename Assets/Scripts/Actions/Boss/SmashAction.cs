using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Boss/Smash")]
public class SmashAction : Action
{
    [SerializeField] private int attackLength = 3;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        return room.GetNeighbors(startLocation, true);
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        Vector3Int forward = targetLocation - entity.location;
        Vector3Int left = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.forward));
        Vector3Int right = Vector3Int.RoundToInt(Vector3.Cross(forward, Vector3Int.back));

        Vector3Int startMiddle = targetLocation;
        Vector3Int startLeft = targetLocation + left;
        Vector3Int startRight = targetLocation + right;

        targets.Add(startMiddle);
        targets.Add(startMiddle + forward);
        targets.Add(startMiddle + forward + left);
        targets.Add(startMiddle + forward + right);

        targets.Add(startMiddle + forward + forward);
        targets.Add(startMiddle + forward + forward + left);
        targets.Add(startMiddle + forward + forward + right);
        targets.Add(startMiddle + forward + forward + left + left);
        targets.Add(startMiddle + forward + forward + right + right);

        return targets;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        foreach (var location in threatenedLocations)
        {
            var target = room.GetEntityAtLocation(location);
            if (target != null)
            {
                entity.Attack(target);
            }
        }

        // Trigger event
        GameEvents.instance.TriggerOnEntityUseWeapon(entity, weapon);

        yield return null;
    }
}
