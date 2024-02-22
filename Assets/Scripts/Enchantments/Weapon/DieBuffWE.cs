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
                if (maxBonus > 0)
                {
                    GrantMax(action);
                }
                else if (minBonus > 0)
                {
                    GrantMin(action);
                }
                else throw new System.Exception("Max and Min are both unset.");

            }
            else if (buffMovementDice && action.actionType == ActionType.Movement)
            {
                if (maxBonus > 0)
                {
                    GrantMax(action);
                }
                else if (minBonus > 0)
                {
                    GrantMin(action);
                }
                else throw new System.Exception("Max and Min are both unset.");
            }
        }
    }

    private void GrantMax(Action action)
    {
        action.die.bonusMaxRoll += maxBonus;

        ModifierTag modifier = new ModifierTag($"+{maxBonus} maximum range", name);
        action.AddOrOverwriteModifier(modifier);
    }

    private void GrantMin(Action action)
    {
        action.die.bonusMinRoll += minBonus;

        ModifierTag modifier = new ModifierTag($"+{minBonus} minimum range", name);
        action.AddOrOverwriteModifier(modifier);
    }

    public override void Uninitialize(Weapon weapon)
    {
        foreach (var action in weapon.actions)
        {
            if (buffAttackDice && action.actionType == ActionType.Attack)
            {
                action.die.bonusMaxRoll -= maxBonus;
                action.die.bonusMinRoll -= minBonus;

                action.RemoveModifier(name);
            }
            else if (buffMovementDice && action.actionType == ActionType.Movement)
            {
                action.die.bonusMaxRoll -= maxBonus;
                action.die.bonusMinRoll -= minBonus;

                action.RemoveModifier(name);
            }
        }
    }
}
