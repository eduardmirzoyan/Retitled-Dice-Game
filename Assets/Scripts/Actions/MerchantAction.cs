using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Merchant")]
public class MerchantAction : Action
{
    private enum Type { Shop, Blacksmith, Shaman }
    [SerializeField] private Type type;

    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        // Target tile directly below
        return new List<Vector3Int>() { startLocation + Vector3Int.down };
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        return new List<Vector3Int>() { targetLocation };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        var target = room.GetEntityAtLocation(targetLocation);
        if (target is Player)
        {
            // Open respective merchant
            switch (type)
            {
                case Type.Shop:

                    // Let player browse the shop
                    yield return ShopUI.instance.Browse(target, entity.inventory);

                    break;
                case Type.Blacksmith:

                    // Let player browse the shop
                    yield return BlacksmithUI.instance.Browse(target);

                    break;
                case Type.Shaman:
                    // TODO
                    break;
            }
        }

        yield return null;
    }
}
