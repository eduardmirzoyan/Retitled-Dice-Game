using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/Range Damage Buff")]
public class RangeDamageWE : WeaponEnchantment
{
    [SerializeField] private int damageBonus = 1;
    [SerializeField] private int range = 2;
    [SerializeField] private int quantity = 2;
    [SerializeField] private bool checkForLongRange;

    private Entity entity;
    public override void Initialize(Weapon weapon)
    {
        this.weapon = weapon;

        GameEvents.instance.onEntityMove += CheckNumberInRange;
    }

    public override void Uninitialize(Weapon weapon)
    {
        GameEvents.instance.onEntityMove -= CheckNumberInRange;
    }

    private void CheckNumberInRange(Entity entity)
    {
        if (checkForLongRange)
        {
            int count = 0;
            foreach (var enemy in entity.room.allEntities)
            {
                if (Room.ManhattanDistance(entity.location, entity.location) > range)
                {
                    count++;
                }
            }

            if (count >= quantity)
            {
                weapon.bonusDamage = damageBonus;
            }
            else
            {
                weapon.bonusDamage = 0;
            }
        }
        else
        {
            int count = 0;
            foreach (var enemy in entity.room.allEntities)
            {
                if (Room.ManhattanDistance(entity.location, entity.location) <= range)
                {
                    count++;
                }
            }

            if (count >= quantity)
            {
                weapon.bonusDamage = damageBonus;
            }
            else
            {
                weapon.bonusDamage = 0;
            }
        }
    }
}
