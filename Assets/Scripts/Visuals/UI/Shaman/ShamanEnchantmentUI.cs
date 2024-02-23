using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ShamanEnchantmentUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Static Data")]
    [SerializeField] private TextMeshProUGUI enchantmentNameText;
    [SerializeField] private TextMeshProUGUI enchantmentRarityText;
    [SerializeField] private TextMeshProUGUI enchantmentDescriptionText;

    [SerializeField] private GameObject FilledHolder;
    [SerializeField] private GameObject EmptyHolder;

    [Header("Settings")]
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color selectedColor;

    [Header("Debug")]
    [SerializeField] private ShamanUI shamanUI;

    public void Initialize(WeaponEnchantment enchantment, bool isSource, int index, ShamanUI shamanUI)
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Notify UI
        }
    }
}
