using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyGenerator : ScriptableObject
{
    public List<Entity> possibleEnemies;
    public Entity shopkeeper;
    public Core core;

    public ItemGenerator shopItemGenerator;

    public Core GenerateCore()
    {
        // Return a copy
        return (Core) core.Copy();
    }

    public Entity GenerateEnemy()
    {
        // If no possible enemies return null
        if (possibleEnemies.Count == 0) return null;

        // Return a copy
        var copy = possibleEnemies[Random.Range(0, possibleEnemies.Count)].Copy();

        // TODO CHANGE THIS!
        // Make both actions take same die
        // copy.GetActions()[1].die = copy.GetActions()[0].die;

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
        for (int i = 0; i < shopSize; i++)
        {
            // Add a random item
            inventory.AddItemToEnd(shopItemGenerator.GenerateItem());   
        }
        // Set inventory
        copy.inventory = inventory;

        return copy;
    }
}
