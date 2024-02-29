using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Entity/Health")]
public class HealthEntityEnchantment : EntityEnchantment
{
    [SerializeField] private int healthBonus;

    public override void Initialize(Entity entity)
    {
        this.entity = entity;
        entity.maxHealth += healthBonus;
        entity.currentHealth += healthBonus;

        GameEvents.instance.TriggerOnEntityTakeDamage(entity, 0);
    }

    public override void Uninitialize()
    {
        entity.maxHealth -= healthBonus;
        entity.currentHealth -= healthBonus;
        this.entity = null;

        GameEvents.instance.TriggerOnEntityTakeDamage(entity, 0);
    }
}
