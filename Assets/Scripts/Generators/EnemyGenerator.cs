using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Generator/Enemy")]
public class EnemyGenerator : ScriptableObject
{
    [Header("Enemies")]
    public List<Entity> possibleEnemies;
    public List<Entity> bosses;
    public List<Entity> merchants; // 0 - shopkeeper, 1 - blacksmith, 2 shaman, etc

    [Header("Neutrals")]
    public Entity barrel;

    [Header("Merchants")]
    public ItemGenerator shopItemGenerator;
    public WeaponGenerator weaponGenerator;
    public int shopSize;

    public Entity GenerateEnemy(int index = -1)
    {
        // If no possible enemies return null
        if (possibleEnemies.Count == 0) return null;

        // Get random index
        if (index == -1)
            index = Random.Range(0, possibleEnemies.Count);

        // Return a copy
        return possibleEnemies[index].Copy();
    }

    public Entity GenerateShopkeeper()
    {
        // Error check
        if (shopSize < 4)
            throw new System.Exception("Shop must be at least 4 slots.");

        var copy = merchants[0].Copy();

        // Create inventory
        var inventory = ScriptableObject.CreateInstance<Inventory>();
        inventory.Initialize(shopSize);

        // First two are always healing potions
        inventory.AddItemToEnd(shopItemGenerator.healingPotion);
        inventory.AddItemToEnd(shopItemGenerator.healingPotion);

        // Next 2 are random non-healing consumbles
        inventory.AddItemToEnd(shopItemGenerator.GenerateConsumable());
        inventory.AddItemToEnd(shopItemGenerator.GenerateConsumable());

        // Fill the rest with randomly generated weapons
        for (int i = 4; i < shopSize; i++)
            inventory.AddItemToEnd(weaponGenerator.GenerateWeapon());

        // Set inventory
        copy.inventory = inventory;

        return copy;
    }

    public Entity GenerateBlacksmith()
    {
        return merchants[1].Copy();
    }

    public Entity GenerateBoss()
    {
        // If no possible enemies return null
        if (bosses.Count == 0) return null;

        // Return a copy of boss based on stage
        return bosses[Random.Range(0, bosses.Count)].Copy();
    }
}
