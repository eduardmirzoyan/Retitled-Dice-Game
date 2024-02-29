using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Generator/Enity Enchantments")]
public class EntityEnchantmentGenerator : ScriptableObject
{
    public List<EntityEnchantment> possibleEnchantments;
    public int numChoices = 3;

    public List<EntityEnchantment> GenerateEnchantmentSet()
    {
        List<EntityEnchantment> list = new();

        for (int i = 0; i < numChoices; i++)
        {
            var enchantment = possibleEnchantments[Random.Range(0, possibleEnchantments.Count)].Copy();
            list.Add(enchantment);
        }

        return list;
    }
}
