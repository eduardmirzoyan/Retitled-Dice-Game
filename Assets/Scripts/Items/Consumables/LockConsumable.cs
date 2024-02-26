using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/Lock")]
public class LockConsumable : Consumable
{
    public override bool CanUse(Entity entity)
    {
        // Always available
        return true;
    }

    public override IEnumerator Use(Entity entity)
    {
        foreach (var action in entity.AllActions())
        {
            if (!action.die.isExhausted)
                action.die.Lock();
        }

        yield return null;
    }
}
