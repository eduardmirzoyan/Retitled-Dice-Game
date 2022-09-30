using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyGenerator : ScriptableObject
{
    public List<Entity> possibleEnemies;

    public Entity GenerateEnemy() {
        // If no possible enemies return null
        if (possibleEnemies.Count == 0) return null;

        // Return a copy
        return possibleEnemies[Random.Range(0, possibleEnemies.Count)].Copy();
    }
}
