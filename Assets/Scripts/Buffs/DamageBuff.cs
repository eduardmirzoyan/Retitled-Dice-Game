using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Damage")]
public class DamageBuff : Buff
{
    [Header("Static Data")]
    [SerializeField] private int bonusAmount = 1;

    public override void Initialize(Action action, string source)
    {
        this.action = action;
        this.source = source;
        action.bonusDamage += bonusAmount * stacks;
    }

    public override void Stack(Buff buff)
    {
        action.bonusDamage += bonusAmount * buff.stacks;
        stacks += buff.stacks;
    }

    public override void Uninitialize()
    {
        action.bonusDamage -= bonusAmount * stacks;
        action = null;
    }

    public override string GetDescription()
    {
        return $"+{bonusAmount * stacks} Damage [{source}]";
    }
}
