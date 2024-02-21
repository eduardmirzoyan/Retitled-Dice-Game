using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnchantmentRarity { Common, Uncommon, Rare }

public abstract class WeaponEnchantment : ScriptableObject
{
    [Header("Static Data")]
    public new string name;
    [TextArea(5, 2)] public string description;
    public EnchantmentRarity rarity;

    [Header("Dynamic Data")]
    public Weapon weapon;

    public abstract void Initialize(Weapon weapon);
    public abstract void Uninitialize(Weapon weapon);

    public WeaponEnchantment Copy()
    {
        return Instantiate(this);
    }
}

[System.Serializable]
public struct ModifierTag
{
    public string text;
    public string source;

    public ModifierTag(string text, string source)
    {
        this.text = text;
        this.source = source;
    }

    public override readonly string ToString()
    {
        return $"{text} [{source}]";
    }
}
