using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite sprite;
    public int value;

    public int GetValue()
    {
        // Change this function as you add dynamic item generation
        return value;
    }

    public virtual Item Copy()
    {
        // Return copy of self
        return Instantiate(this);
    }
}
