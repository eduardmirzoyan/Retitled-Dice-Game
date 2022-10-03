using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShopkeeperAI : AI
{
    public override (Action, Vector3Int) GenerateBestDecision(Entity entity, Room dungeon)
    {
        // Only consider the first action of the entity
        var moveAction = entity.actions[0];
        var shopAction = entity.actions[1];

        // First use Shop action if player is in range
        foreach (var location in shopAction.GetValidLocations(entity.location, dungeon))
        {
            // If the action is on the player
            if (location == dungeon.player.location) {
                // Choose this!
                return (shopAction, location);
            }
        }

        // Then if nothing hit, randomly use Move action randomly
        var locations = moveAction.GetValidLocations(entity.location, dungeon);
        // If no possible move, return Pass action
        if (locations.Count == 0) return (null, Vector3Int.zero);

        var randomLocation = locations[Random.Range(0, locations.Count)];

        return (moveAction, randomLocation);
    }
}
