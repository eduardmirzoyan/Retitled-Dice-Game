using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Boss/Toss")]
public class TossAction : Action
{
    [SerializeField] private int radius = 1;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        // Simply target the player
        return new List<Vector3Int>() { room.player.location };
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        for (int i = targetLocation.x - radius; i <= targetLocation.x + radius; i++)
        {
            for (int j = targetLocation.y - radius; j <= targetLocation.y + radius; j++)
            {
                var location = new Vector3Int(i, j, 0);
                if (location != targetLocation)
                    targets.Add(location);
            }
        }

        return targets;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        foreach (var location in threatenedLocations)
        {
            var target = room.GetEntityAtLocation(location);
            if (target != null)
            {
                entity.AttackEntity(target, weapon, GetTotalDamage());
            }
        }

        // Trigger event
        GameEvents.instance.TriggerOnEntityUseWeapon(entity, weapon);

        yield return null;
    }

}
