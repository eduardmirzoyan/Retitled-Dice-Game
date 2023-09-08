using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WeaponEnchantmentTooltipUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private TextMeshProUGUI enchantmentNameText;
    [SerializeField] private TextMeshProUGUI enchantmentDescriptionText;

    public void Initialize(WeaponEnchantment enchantment)
    {
        // Set text
        enchantmentNameText.text = enchantment.name;
        enchantmentDescriptionText.text = enchantment.description;
    }
}
