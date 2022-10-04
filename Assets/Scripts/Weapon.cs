using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : Item
{
    public GameObject attackParticlePrefab;
    public List<Action> actions;

    public Weapon Copy()
    {
        var copy = Instantiate(this);

        // Make a copy of each action
        for (int i = 0; i < actions.Count; i++)
        {
            copy.actions[i] = actions[i].Copy();
        }

        return copy;
    }
}
