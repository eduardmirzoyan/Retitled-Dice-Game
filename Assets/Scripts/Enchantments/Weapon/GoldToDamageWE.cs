using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/Gold To Damage")]
public class GoldToDamageWE : WeaponEnchantment
{
    [Header("Settings")]
    [SerializeField] private int numGoldToOneDamage = 5;

    [Header("Debug")]
    [SerializeField] private int bonusGranted;

    public override void Initialize(Weapon weapon)
    {
        this.weapon = weapon;
        GameEvents.instance.onEntityGoldChange += UpdateDamageBonus;
        bonusGranted = 0;

        UpdateDamageBonus(weapon.entity, 0);
    }

    public override void Uninitialize(Weapon weapon)
    {
        foreach (var action in weapon.actions)
        {
            if (action.actionType == ActionType.Attack)
                action.RemoveModifier(name);
        }

        this.weapon = null;
        GameEvents.instance.onEntityGoldChange -= UpdateDamageBonus;
        bonusGranted = 0;
    }

    private void UpdateDamageBonus(Entity entity, int _)
    {
        // When the holder gains gold
        if (weapon.entity == entity)
        {
            foreach (var action in weapon.actions)
            {
                if (action.actionType == ActionType.Attack)
                {
                    // Remove previous bonuses
                    if (bonusGranted > 0)
                    {
                        action.bonusDamage -= bonusGranted;
                    }

                    // Calculate new bonus and apply
                    bonusGranted = entity.gold / numGoldToOneDamage;

                    action.bonusDamage += bonusGranted;

                    // Create tag
                    ModifierTag modifier = new ModifierTag($"+{bonusGranted} damage", name);
                    action.AddOrOverwriteModifier(modifier);
                }
            }
        }
    }
}
