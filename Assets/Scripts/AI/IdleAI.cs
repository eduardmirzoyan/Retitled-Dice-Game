using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Idle")]
public class IdleAI : AI
{
    public override List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity)
    {
        // Does nothing.
        return new List<(Action, Vector3Int)>();
    }
}
