using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Shop")]
public class ShopAction : Action
{
    public override List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room)
    {
        // Target tile directly bellow
        return new List<Vector3Int>() { startLocation + Vector3Int.down };
    }

    public override List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation)
    {
        return new List<Vector3Int>() { targetLocation };
    }

    public override IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room)
    {
        // Logic
        foreach (var location in threatenedLocations)
        {
            // Damage first target found
            var target = room.GetEntityAtLocation(location);
            if (target is Player)
            {
                // Open shop
                GameEvents.instance.TriggerOnOpenShop(target, target.inventory);

                // Dip
                break;
            }
        }

        yield return null;
    }
}
