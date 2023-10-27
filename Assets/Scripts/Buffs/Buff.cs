using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : ScriptableObject
{
    [Header("Dynamic Data")]
    public int stacks = 1;
    public Action action;
    public string source;

    public abstract void Initialize(Action action, string source);
    public abstract void Stack(Buff buff);
    public abstract void Uninitialize();
    public abstract string GetDescription();

    public Buff Copy()
    {
        return Instantiate(this);
    }
}
