using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Entity/None")]
public class NoneEntityEnchantment : EntityEnchantment
{
    public override void Initialize(Entity entity)
    {
        Debug.Log("Enabled Entity Enchantment: " + name);
    }

    public override void Uninitialize()
    {
        Debug.Log("Disabled Entity Enchantment: " + name);
    }
}
