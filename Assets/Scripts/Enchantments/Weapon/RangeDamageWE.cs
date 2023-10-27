using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/Range Damage Buff")]
public class RangeDamageWE : WeaponEnchantment
{
    [Header("Static Data")]
    [SerializeField] private int damageBonus = 1;
    [SerializeField] private int range = 2;
    [SerializeField] private int quantity = 2;
    [SerializeField] private bool checkForLongRange;

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
                if (entity != enemy && Room.ManhattanDistance(entity.location, enemy.location) > range)
                {
                    count++;
                }
            }

            if (count >= quantity)
            {
                weapon.baseDamage = damageBonus;
            }
            else
            {
                weapon.baseDamage = 0;
            }
        }
        else
        {
            int count = 0;
            foreach (var enemy in entity.room.allEntities)
            {
                if (entity != enemy && Room.ManhattanDistance(entity.location, enemy.location) <= range)
                {
                    count++;
                }
            }

            if (count >= quantity)
            {
                Debug.Log("Buff Gain!");
                weapon.baseDamage = damageBonus;
            }
            else
            {
                Debug.Log("Buff Lost!");
                weapon.baseDamage = 0;
            }
        }
    }
}
