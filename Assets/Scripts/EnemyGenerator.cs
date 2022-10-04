using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyGenerator : ScriptableObject
{
    public List<Entity> possibleEnemies;
    public Entity shopkeeper;

    public Entity GenerateEnemy()
    {
        // If no possible enemies return null
        if (possibleEnemies.Count == 0) return null;

        // Return a copy
        var copy = possibleEnemies[Random.Range(0, possibleEnemies.Count)].Copy();

        // TODO CHANGE THIS!
        // Make both actions take same die
        copy.innateActions[1].die = copy.innateActions[0].die;

        return copy;
    }

    public Entity GenerateShopkeeper()
    {
        
        return shopkeeper.Copy();
    }
}
