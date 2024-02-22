using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/Gold On Kill")]
public class KillGoldWeaponEnchantment : WeaponEnchantment
{
    [SerializeField] private int goldOnKill = 1;

    public override void Initialize(Weapon weapon)
    {
        this.weapon = weapon;
        GameEvents.instance.onEntityKillEntity += GrantGold;

        ModifierTag modifier = new ModifierTag($"On Kill: +{goldOnKill} Gold", name);
        foreach (var action in weapon.actions)
        {
            if (action.actionType == ActionType.Attack)
                action.AddOrOverwriteModifier(modifier);
        }
    }

    public override void Uninitialize(Weapon weapon)
    {
        foreach (var action in weapon.actions)
        {
            if (action.actionType == ActionType.Attack)
                action.RemoveModifier(name);
        }

        this.weapon = null;
        GameEvents.instance.onEntityKillEntity -= GrantGold;
    }

    private void GrantGold(Entity killer, Weapon weapon, Entity victim)
    {
        // If this weapon killed someone
        if (this.weapon == weapon)
        {
            // Give extra gold to killer
            killer.AddGold(goldOnKill);
        }
    }
}
