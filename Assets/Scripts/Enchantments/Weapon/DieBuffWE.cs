using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/Die Buff")]
public class DieBuffWE : WeaponEnchantment
{
    [Header("Affected Action Types")]
    [SerializeField] private bool buffAttackDice;
    [SerializeField] private bool buffMovementDice;

    [Header("Buffs")]
    [SerializeField] private int maxBonus;
    [SerializeField] private int minBonus;

    public override void Initialize(Weapon weapon)
    {
        foreach (var action in weapon.actions)
        {
            if (buffAttackDice && action.actionType == ActionType.Attack)
            {
                action.die.maxValue += maxBonus;
                action.die.minValue += minBonus;
            }
            else if (buffMovementDice && action.actionType == ActionType.Movement)
            {
                action.die.maxValue += maxBonus;
                action.die.minValue += minBonus;
            }
        }
    }

    public override void Uninitialize(Weapon weapon)
    {
        foreach (var action in weapon.actions)
        {
            if (buffAttackDice && action.actionType == ActionType.Attack)
            {
                action.die.maxValue -= maxBonus;
                action.die.minValue -= minBonus;
            }
            else if (buffMovementDice && action.actionType == ActionType.Movement)
            {
                action.die.maxValue -= maxBonus;
                action.die.minValue -= minBonus;
            }
        }
    }

}
