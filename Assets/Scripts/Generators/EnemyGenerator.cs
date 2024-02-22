using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Generator/Enemy")]
public class EnemyGenerator : ScriptableObject
{
    public List<Entity> possibleEnemies;
    public List<Entity> bosses;
    public List<Entity> merchants; // 0 - shopkeeper, 1 - blacksmith, 2 shaman, etc
    public Entity barrel;

    public int shopSize;

    public ItemGenerator shopItemGenerator;
    public WeaponGenerator weaponGenerator;

    public Entity GenerateEnemy(int index = -1)
    {
        // If no possible enemies return null
        if (possibleEnemies.Count == 0) return null;

        if (index == -1)
            index = Random.Range(0, possibleEnemies.Count);

        // Return a copy
        return possibleEnemies[index].Copy();
    }

    public Entity GenerateShopkeeper()
    {
        var copy = merchants[0].Copy();

        // Create inventory
        var inventory = ScriptableObject.CreateInstance<Inventory>();
        inventory.Initialize(shopSize);

        // Fill inventory
        inventory.AddItemToEnd(shopItemGenerator.healingPotion);
        for (int i = 0; i < shopSize - 1; i++)
        {
            // Add a random item
            inventory.AddItemToEnd(weaponGenerator.GenerateWeapon());
        }
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
