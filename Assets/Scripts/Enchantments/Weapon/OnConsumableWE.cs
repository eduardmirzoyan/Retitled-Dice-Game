using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/On Consume")]
public class OnConsumableWE : WeaponEnchantment
{
    [Header("Static Data")]
    [SerializeField] private Buff buff;

    [Header("Debugging")]
    [SerializeField] private bool isApplied;

    public override void Initialize(Weapon weapon)
    {
        this.weapon = weapon;
        GameEvents.instance.onEntityUseConsumable += ApplyBuff;
        GameEvents.instance.onTurnEnd += RemoveBuff;
    }

    public override void Uninitialize(Weapon weapon)
    {
        this.weapon = null;
        GameEvents.instance.onEntityUseConsumable -= ApplyBuff;
        GameEvents.instance.onTurnEnd -= RemoveBuff;
    }

    private void ApplyBuff(Entity entity, Consumable consumable)
    {
        if (!isApplied && entity.weapons.Contains(weapon))
        {
            foreach (var action in weapon.actions)
            {
                if (action.actionType == ActionType.Attack)
                {
                    action.AddBuff(buff, name);
                }
            }
        }
    }

    private void RemoveBuff(Entity entity)
    {
        if (isApplied && entity.weapons.Contains(weapon))
        {
            foreach (var action in weapon.actions)
            {
                if (action.actionType == ActionType.Attack)
                {
                    action.RemoveBuff(buff, name);
                }
            }
        }
    }
}
