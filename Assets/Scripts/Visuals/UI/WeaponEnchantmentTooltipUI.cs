using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponEnchantmentTooltipUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private TextMeshProUGUI enchantmentNameText;
    [SerializeField] private TextMeshProUGUI enchantmentRarityText;
    [SerializeField] private TextMeshProUGUI enchantmentDescriptionText;

    public void Initialize(WeaponEnchantment enchantment)
    {
        // Set text
        enchantmentNameText.text = enchantment.name;
        enchantmentRarityText.text = $"{enchantment.rarity} Enchantment";
        enchantmentDescriptionText.text = enchantment.description;
    }
}
