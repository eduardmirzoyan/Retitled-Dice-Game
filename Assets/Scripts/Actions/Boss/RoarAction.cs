using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Boss/Roar")]
public class RoarAction : Action
{
    [SerializeField] private int radius = 1;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        return new List<Vector3Int>() { startLocation };
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        for (int i = targetLocation.x - radius; i <= targetLocation.x + radius; i++)
        {
            for (int j = targetLocation.y - radius; j <= targetLocation.y + radius; j++)
            {
                targets.Add(new Vector3Int(i, j, 0));
            }
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
