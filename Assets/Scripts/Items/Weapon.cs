using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : Item
{
    [Header("Static Data")]
    public int baseDamage = 1;
    public List<Action> actions;
    public List<WeaponEnchantment> enchantments;
    public int enchantmentCap = 3;

    [Header("Visuals")]
    public GameObject attackParticlePrefab;
    public GameObject weaponPrefab;

    [Header("Dynamic Data")]
    public Entity entity;

    public void Initialize(Entity entity)
    {
        this.entity = entity;

        foreach (var actions in actions)
        {
            actions.Initialize(this);
        }

        foreach (var enchantment in enchantments)
        {
            if (enchantment != null)
                enchantment.Initialize(this);
        }
    }

    public void Uninitialize()
    {
        this.entity = null;

        foreach (var actions in actions)
        {
            actions.Uninitialize();
        }

        foreach (var enchantment in enchantments)
        {
            if (enchantment != null)
                enchantment.Uninitialize(this);
        }
    }

    public int UpgradeCost()
    {
        return baseDamage * 15;
    }

    public void IncreaseBaseDamage()
    {
        baseDamage += 1;

        name += "+";
    }

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



        // Make a copy of each enchantment
        for (int i = 0; i < enchantments.Count; i++)
        {
            // Make copies
            copy.enchantments[i] = enchantments[i].Copy();
            // Change owner
            copy.enchantments[i].weapon = copy;
        }

        // Check if there should be empty slots
        if (enchantments.Count > enchantmentCap)
            throw new System.Exception("More enchantments than allowed.");
        else if (enchantments.Count < enchantmentCap)
        {
            // Make up the diff with empty slots
            for (int i = 0; i < enchantmentCap - enchantments.Count; i++)
            {
                copy.enchantments.Add(null);
            }
        }

        return copy;
    }
}
