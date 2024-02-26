using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/Reroll")]
public class RerollConsumable : Consumable
{
    public override bool CanUse(Entity entity)
    {
        return true;
    }

    public override IEnumerator Use(Entity entity)
    {
        foreach (var action in entity.AllActions())
        {
            if (!action.die.isExhausted)
                action.die.Roll();
        }

        yield return null;
    }
}
