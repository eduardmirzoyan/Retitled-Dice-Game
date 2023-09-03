using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class HealingPotion : Consumable
{
    public int healAmount = 1;

    public override bool Use(Entity entity)
    {
        if (entity.currentHealth >= entity.maxHealth) return false;

        // Heal entity
        entity.Heal(healAmount);

        // Debug
        Debug.Log("Healed " + entity.name + " " + healAmount + " HP.");

        return true;
    }
}

