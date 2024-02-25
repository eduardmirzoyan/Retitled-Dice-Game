using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/Lock")]
public class LockConsumable : Consumable
{
    public override bool CanUse(Entity entity)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Use(Entity entity)
    {
        throw new System.NotImplementedException();
    }
}
