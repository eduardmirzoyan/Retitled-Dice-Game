using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : Item
{
    public GameObject attackParticlePrefab;
    public List<Action> actions;
    public RuntimeAnimatorController controller;

    public Weapon Copy()
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
