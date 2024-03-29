using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/Bump")]
public class BumpConsumable : Consumable
{
    [SerializeField] private int bumpValue = 1;

    public override bool CanUse(Entity entity)
    {
        return true;
    }

    public override IEnumerator Use(Entity entity)
    {
        foreach (var action in entity.AllActions())
        {
            if (!action.die.isExhausted)
                action.die.Bump(bumpValue);
        }

        yield return null;
    }
}
