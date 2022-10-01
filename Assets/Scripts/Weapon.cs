using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite sprite;
    public GameObject attackParticlePrefab;

    // Does nothing for now...

    public Weapon Copy() {
        var copy = Instantiate(this);

        // Fill in as class expands...

        return copy;
    }
}
