using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Consumables/Heal")]
public class HealingConsumable : Consumable
{
    public int healAmount = 1;

    public override bool CanUse(Entity entity)
    {
        // Error checking
        if (entity == null) throw new System.Exception("Entity was null.");

        return entity.currentHealth < entity.maxHealth;
    }

    public override IEnumerator Use(Entity entity)
    {
        // Heal entity
        entity.Heal(healAmount);

        yield return null;
    }
}

