using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Idle")]
public class IdleAI : AI
{
    /*
    Overview: Tries to get close as close as possible to the player, aims towards the player
    */
    public override List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity)
    {
        // Does nothing.
        return new List<(Action, Vector3Int)>();
    }
}
