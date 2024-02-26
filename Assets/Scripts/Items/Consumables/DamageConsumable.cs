using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/Damge")]
public class DamageConsumable : Consumable
{
    [SerializeField] private int damageBoost = 1;

    private Entity entity;

    public override bool CanUse(Entity entity)
    {
        return true;
    }

    public override IEnumerator Use(Entity entity)
    {
        this.entity = entity;

        // Buff actions
        ModifierTag modifier = new($"+{damageBoost} damage", name);
        foreach (var action in entity.AllActions())
        {
            if (action.actionType == ActionType.Attack)
            {
                action.bonusDamage += damageBoost;
                action.AddOrOverwriteModifier(modifier);
            }
        }

        // Sub
        GameEvents.instance.onTurnEnd += RemoveBoost;

        yield return null;
    }

    private void RemoveBoost(Entity entity)
    {
        if (this.entity == entity)
        {
            // Remove buff and tag
            foreach (var action in entity.AllActions())
            {
                if (action.actionType == ActionType.Attack)
                {
                    action.bonusDamage -= damageBoost;
                    action.RemoveModifier(name);
                }
            }

            // Unsub
            GameEvents.instance.onTurnEnd -= RemoveBoost;
        }
    }
}
