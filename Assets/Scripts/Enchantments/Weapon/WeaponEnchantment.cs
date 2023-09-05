using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponEnchantment : ScriptableObject
{
    [Header("Static Data")]
    public new string name;
    [TextArea(5, 2)] public string description;

    [Header("Dynamic Data")]
    public Weapon weapon;

    public abstract void Initialize(Weapon weapon);
    public abstract void Uninitialize(Weapon weapon);

    public WeaponEnchantment Copy()
    {
        return Instantiate(this);
    }
}
