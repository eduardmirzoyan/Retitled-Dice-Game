using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShopkeeperAI : AI
{
    public override void DisplayIntent(Entity entity, Room room)
    {
        var shopAction = entity.GetActions()[1];

        // Trigger event to display targets
        GameEvents.instance.TriggerOnActionSelect(entity, shopAction, room);
    }

    public override (Action, Vector3Int) GenerateBestDecision(Entity entity, Room room)
    {
        // Only consider the first action of the entity
        var moveAction = entity.GetActions()[0];
        var shopAction = entity.GetActions()[1];

        // First use Shop action if player is in range
        foreach (var location in shopAction.GetValidLocations(entity.location, room))
        {
            // If the action is on the player
            if (location == room.player.location) {
                // Choose this!
                return (shopAction, location);
            }
        }

        // Then if nothing hit, randomly use Move action randomly
        var locations = moveAction.GetValidLocations(entity.location, room);
        // If no possible move, return Pass action
        if (locations.Count == 0) return (null, Vector3Int.zero);

        var randomLocation = locations[Random.Range(0, locations.Count)];

        return (moveAction, randomLocation);
    }
}
