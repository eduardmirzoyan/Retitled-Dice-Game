using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Merchant")]
public class MerchantAI : AI
{
    /*
    Overview: Tries to shop with player infront of it
    */
    public override List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity)
    {
        var actionPairSquence = new List<(Action, Vector3Int)>();

        // Get all possible actions
        var possibleActions = entity.GetActions();

        // ASSUME FIRST ACTION IS A SHOP ACTION
        var shopAction = possibleActions[0];
        if (shopAction is not MerchantAction)
            throw new System.Exception("MERCHANT DOES NOT HAVE MERCHANT ACTION.");

        // Get first location in possible actions
        var shopLocation = shopAction.GetValidLocations(entity.location, room)[0];

        // Add pair
        actionPairSquence.Add((shopAction, shopLocation));

        return actionPairSquence;
    }
}
