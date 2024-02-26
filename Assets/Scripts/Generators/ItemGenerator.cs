using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Generator/Item")]
public class ItemGenerator : ScriptableObject
{
    public List<Item> possibleItems;

    public HealingConsumable healingPotion;
    public List<Consumable> possibleConsumables;

    public Item GenerateItem()
    {
        // Randomly choose item
        int randomIndex = Random.Range(0, possibleItems.Count);

        // Return a copy of item at index
        return possibleItems[randomIndex].Copy();
    }

    public Consumable GenerateConsumable()
    {
        // Randomly choose item
        int randomIndex = Random.Range(0, possibleConsumables.Count);

        // Return a copy of item at index
        return possibleConsumables[randomIndex].Copy() as Consumable;
    }
}
