using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/Warp")]
public class WarpPotion : Consumable
{
    // Find a random, empty location in room and warp to it
    public override bool Use(Entity entity)
    {
        var tiles = entity.room.tiles;

        int totalTries = 20;
        while (totalTries > 0)
        {
            int randomX = Random.Range(0, tiles.GetLength(0));
            int randomY = Random.Range(0, tiles.GetLength(1));

            Vector3Int location = new(randomX, randomY);

            if (!entity.room.IsWall(location) && !entity.room.IsChasam(location) && !entity.room.HasEntity(location))
            {
                Debug.Log($"{entity} warped to {location}.");

                // entity.WarpTo(location);

                return true;
            }

            totalTries--;
        }

        Debug.Log($"{entity} could not find valid tile.");

        return false;
    }
}
