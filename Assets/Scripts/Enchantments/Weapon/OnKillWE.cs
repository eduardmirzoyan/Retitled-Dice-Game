using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/On Kill")]
public class OnKillWE : WeaponEnchantment
{
    // FIXME

    public override void Initialize(Weapon weapon)
    {
        this.weapon = weapon;
        GameEvents.instance.onEntityKillEntity += ApplyBuff;
        GameEvents.instance.onTurnEnd += RemoveBuff;
    }

    public override void Uninitialize(Weapon weapon)
    {
        this.weapon = null;
        GameEvents.instance.onEntityKillEntity -= ApplyBuff;
        GameEvents.instance.onTurnEnd -= RemoveBuff;
    }

    private void ApplyBuff(Entity killer, Weapon weapon, Entity victim)
    {
        // If this weapon killed
        if (this.weapon == weapon)
        {
            foreach (var action in weapon.actions)
            {
                if (action.actionType == ActionType.Attack)
                {
                    // Give buff
                    // TODO
                    break;
                }
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
