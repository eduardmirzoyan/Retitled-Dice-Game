using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/None")]
public class NoneBuff : Buff
{
    public override void Initialize(Action action, string source)
    {
        Debug.Log("Initialize None Buff");
    }

    public override void Stack(Buff buff)
    {
        Debug.Log("Stacked None Buff");
    }

    public override void Uninitialize()
    {
        Debug.Log("Uninitialize None Buff");
    }

    public override string GetDescription()
    {
        return "+Nothing";
    }
}
