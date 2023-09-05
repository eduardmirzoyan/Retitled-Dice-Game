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
    }

    public override void Uninitialize(Weapon weapon)
    {
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
