using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/Warp")]
public class WarpConsumable : Consumable
{
    public override bool CanUse(Entity entity)
    {
        foreach (var tile in entity.room.tiles)
        {
            // If there is a single open tile, then we are good
            Vector3Int location = tile.location;
            if (!entity.room.IsWall(location) && !entity.room.IsChasam(location) && !entity.room.HasEntity(location))
                return true;
        }

        return false;
    }

    // Find a random, empty location in room and warp to it
    public override IEnumerator Use(Entity entity)
    {
        List<Vector3Int> openLocations = new List<Vector3Int>();

        // Find all valid locations
        Vector3Int location;
        foreach (var tile in entity.room.tiles)
        {
            // If there is a single open tile, then we are good
            location = tile.location;
            if (!entity.room.IsWall(location) && !entity.room.IsChasam(location) && !entity.room.HasEntity(location))
                openLocations.Add(location);
        }

        // Error check
        if (openLocations.Count == 0)
            throw new System.Exception("No open locations found.");

        // Randomly choose one from list
        location = openLocations[Random.Range(0, openLocations.Count)];

        // Debug
        Debug.Log($"{entity} warped to {location}.");

        // Handle visuals
        yield return entity.model.Warp(entity.location, location);

        // Relocate
        entity.Relocate(location);
    }
}
