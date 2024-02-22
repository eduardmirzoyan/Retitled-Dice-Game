using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Generator/Item")]
public class ItemGenerator : ScriptableObject
{
    public List<Item> possibleItems;
    public HealingPotion healingPotion;

    public Item GenerateItem()
    {
        // Randomly choose item
        int randomIndex = Random.Range(0, possibleItems.Count);

        // Return a copy of item at index
        return possibleItems[randomIndex].Copy();
    }
}
