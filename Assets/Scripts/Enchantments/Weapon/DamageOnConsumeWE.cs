using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/Damage On Consume")]
public class DamageOnConsumeWE : WeaponEnchantment
{
    [SerializeField] private int damageBonus = 1;

    public override void Initialize(Weapon weapon)
    {
        this.weapon = weapon;
        GameEvents.instance.onEntityUseConsumable += GrantDamage;
    }

    public override void Uninitialize(Weapon weapon)
    {
        this.weapon = null;
        GameEvents.instance.onEntityUseConsumable -= GrantDamage;
    }

    private void GrantDamage(Entity entity, Consumable consumable)
    {
        if (entity.weapons.Contains(weapon))
        {
            // weapon.bonusDamage += damageBonus;

            // TODO
        }
    }
}
