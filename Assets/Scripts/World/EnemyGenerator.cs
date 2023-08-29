using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyGenerator : ScriptableObject
{
    public List<Entity> possibleEnemies;
    public List<Entity> bosses;
    public Entity shopkeeper;
    public Barrel barrel;

    public ItemGenerator shopItemGenerator;

    public Entity GenerateEnemy()
    {
        // If no possible enemies return null
        if (possibleEnemies.Count == 0) return null;

        // Return a copy
        var copy = possibleEnemies[Random.Range(0, possibleEnemies.Count)].Copy();

        return copy;
    }

    public Entity GenerateShopkeeper()
    {
        var copy = shopkeeper.Copy();

        int shopSize = 4;

        // Create inventory
        var inventory = ScriptableObject.CreateInstance<Inventory>();
        inventory.Initialize(shopSize);

        // Fill inventory
        inventory.AddItemToEnd(shopItemGenerator.healingPotion);
        for (int i = 0; i < shopSize - 1; i++)
        {
            // Add a random item
            inventory.AddItemToEnd(shopItemGenerator.GenerateItem());
        }
        // Set inventory
        copy.inventory = inventory;

        return copy;
    }

    public Entity GenerateBoss()
    {
        // If no possible enemies return null
        if (bosses.Count == 0) return null;

        // Return a copy of boss based on stage
        var copy = bosses[Random.Range(0, bosses.Count)].Copy();

        return copy;
    }
}
