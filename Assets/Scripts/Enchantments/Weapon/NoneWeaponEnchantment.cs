using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/None")]
public class NoneWeaponEnchantment : WeaponEnchantment
{
    public override void Initialize(Weapon weapon)
    {
        Debug.Log("Enabled Weapon Enchantment: " + name);
    }

    public override void Uninitialize(Weapon weapon)
    {
        Debug.Log("Disabled Weapon Enchantment: " + name);
    }
}
