using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/On Reroll")]
public class OnRerollWE : WeaponEnchantment
{
    [Header("Debug")]
    [SerializeField] private int numTriggered;

    public override void Initialize(Weapon weapon)
    {
        this.weapon = weapon;
        GameEvents.instance.onDieRoll += ApplyBuff;
        GameEvents.instance.onTurnEnd += RemoveBuff;
    }

    public override void Uninitialize(Weapon weapon)
    {
        this.weapon = null;
        GameEvents.instance.onDieRoll -= ApplyBuff;
        GameEvents.instance.onTurnEnd -= RemoveBuff;
    }

    private void ApplyBuff(Die die)
    {
        // Check if the rerolled die is one of this weapon's
        foreach (var action in weapon.actions)
        {
            if (action.die == die && action.actionType == ActionType.Attack)
            {
                // Apply bonus
                action.bonusDamage += 1;

                // Increment
                numTriggered++;

                // Log changes
                ModifierTag modifier = new ModifierTag($"+{numTriggered} damage", name);
                action.AddOrOverwriteModifier(modifier);

                break;
            }
        }
    }

    private void RemoveBuff(Entity entity)
    {
        // If holder's turn ends
        if (entity.weapons.Contains(weapon))
        {
            foreach (var action in weapon.actions)
            {
                // Remove buff
                // TODO
            }
        }
    }
}
