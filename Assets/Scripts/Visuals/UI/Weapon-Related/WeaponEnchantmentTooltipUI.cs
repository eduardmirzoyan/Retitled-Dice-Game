using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponEnchantmentTooltipUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private TextMeshProUGUI enchantmentNameText;
    [SerializeField] private TextMeshProUGUI enchantmentRarityText;
    [SerializeField] private TextMeshProUGUI enchantmentDescriptionText;

    [SerializeField] private GameObject FilledHolder;
    [SerializeField] private GameObject EmptyHolder;

    public void Initialize(WeaponEnchantment enchantment)
    {
        if (enchantment != null)
        {
            enchantmentNameText.text = enchantment.name;
            enchantmentRarityText.text = $"{enchantment.rarity} Enchantment";
            Color color = ResourceMananger.instance.GetColor(enchantment.rarity);
            enchantmentRarityText.color = color;
            enchantmentDescriptionText.text = enchantment.description;
        }
        else
        {
            EmptyHolder.SetActive(true);
            FilledHolder.SetActive(false);
        }
    }
}
