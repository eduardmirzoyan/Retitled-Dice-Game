using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/On Consume")]
public class OnConsumableWE : WeaponEnchantment
{
    [Header("Debug")]
    [SerializeField] private int numTriggered;

    public override void Initialize(Weapon weapon)
    {
        this.weapon = weapon;
        numTriggered = 0;

        GameEvents.instance.onEntityUseConsumable += Activate;
        GameEvents.instance.onTurnEnd += Deactivate;
    }

    public override void Uninitialize(Weapon weapon)
    {
        this.weapon = null;
        numTriggered = 0;

        GameEvents.instance.onEntityUseConsumable -= Activate;
        GameEvents.instance.onTurnEnd -= Deactivate;
    }

    private void Activate(Entity entity, Consumable consumable)
    {
        if (entity.weapons.Contains(weapon))
        {
            foreach (var action in weapon.actions)
            {
                if (action.actionType == ActionType.Attack)
                {
                    // Apply bonus
                    action.bonusDamage += 1;

                    // Increment
                    numTriggered++;

                    // Log changes
                    ModifierTag modifier = new ModifierTag($"+{numTriggered} damage", name);
                    action.AddOrOverwriteModifier(modifier);
                }
            }
        }
    }

    private void Deactivate(Entity entity)
    {
        if (numTriggered > 0 && entity.weapons.Contains(weapon))
        {
            foreach (var action in weapon.actions)
            {
                if (action.actionType == ActionType.Attack)
                {
                    action.bonusDamage -= numTriggered;
                    numTriggered = 0;

                    action.RemoveModifier(name);
                }
            }
        }
    }
}
