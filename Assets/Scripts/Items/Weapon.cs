using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : Item
{
    [Header("Mechanics")]
    public List<Action> actions;

    [Header("Visuals")]
    public GameObject attackParticlePrefab;
    public GameObject weaponPrefab;

    public override Item Copy()
    {
        var copy = Instantiate(this);

        // Make a copy of each action
        for (int i = 0; i < actions.Count; i++)
        {
            // Make copies
            copy.actions[i] = actions[i].Copy();
            // Change owner
            copy.actions[i].weapon = copy;
        }

        return copy;
    }
}
