using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Alignment { Allied, Neutral, Hostile }

public abstract class AI : ScriptableObject
{
    public Alignment alignment;

    public abstract List<(Action, Vector3Int)> GenerateSequenceOfActions(Entity entity, Room room, Entity targetEntity);
}
