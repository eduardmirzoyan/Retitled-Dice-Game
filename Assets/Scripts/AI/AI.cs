using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/AI")]
public abstract class AI : ScriptableObject
{
    public bool isHostile = true;

    public abstract List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity);
}
