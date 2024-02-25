using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/Bump")]
public class BumpConsumable : Consumable
{
    [SerializeField] private int bumpValue = 1;

    public override bool CanUse(Entity entity)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Use(Entity entity)
    {
        throw new System.NotImplementedException();
    }
}
