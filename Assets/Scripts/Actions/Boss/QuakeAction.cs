using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Boss/Quake")]
public class QuakeAction : Action
{
    [SerializeField] private int radius = 3;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        // Simply target the player
        return new List<Vector3Int>() { room.player.location };
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        targets.Add(targetLocation);
        for (int i = 1; i < radius; i++)
        {
            targets.Add(targetLocation + new Vector3Int(i, i, 0));
            targets.Add(targetLocation + new Vector3Int(-i, i, 0));
            targets.Add(targetLocation + new Vector3Int(i, -i, 0));
            targets.Add(targetLocation + new Vector3Int(-i, -i, 0));
        }

        return targets;
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        foreach (var location in threatenedLocations)
            entity.AttackLocation(location, weapon, GetTotalDamage());

        yield return weapon.model.Attack();
        yield return weapon.model.Sheathe();
    }

}
