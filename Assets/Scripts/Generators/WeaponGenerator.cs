using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Generator/Weapon")]
public class WeaponGenerator : ScriptableObject
{
    [Header("Data")]
    public List<Weapon> possibleWeapons;
    public List<WeaponEnchantment> commonEnchantments;
    public List<WeaponEnchantment> uncommonEnchantments;
    public List<WeaponEnchantment> rareEnchantments;

    [Header("Settings")]
    public int maxPossibleSlots;
    [Range(0, 100)] public int slotChance;
    [Range(0, 100)] public int commonChance;
    [Range(0, 100)] public int uncommonChance;
    [Range(0, 100)] public int rareChance;

    public Weapon GenerateWeapon()
    {
        // Make a copy of a random weapon
        int randomIndex = Random.Range(0, possibleWeapons.Count);
        Weapon weapon = possibleWeapons[randomIndex].Copy() as Weapon;

        System.Random rng = new();

        // First decide how many enchantment slots
        List<WeaponEnchantment> enchantments = new List<WeaponEnchantment>();
        for (int i = 0; i < maxPossibleSlots; i++)
        {
            if (rng.Next(100) > slotChance)
            {
                enchantments.Add(null);
            }
        }

        for (int i = 0; i < enchantments.Count; i++)
        {
            // Then decide which type
            int value = rng.Next(100);
            if (value < commonChance)
            {
                randomIndex = Random.Range(0, commonEnchantments.Count);
                enchantments[i] = commonEnchantments[randomIndex].Copy();
            }
            else if (value < commonChance + uncommonChance)
            {
                randomIndex = Random.Range(0, uncommonEnchantments.Count);
                enchantments[i] = uncommonEnchantments[randomIndex].Copy();
            }
            else
            {
                randomIndex = Random.Range(0, rareEnchantments.Count);
                enchantments[i] = rareEnchantments[randomIndex].Copy();
            }
        }

        // Finally decide on each individual rarity
        weapon.enchantmentCap = maxPossibleSlots;
        weapon.enchantments = enchantments;

        return weapon;
    }
}
