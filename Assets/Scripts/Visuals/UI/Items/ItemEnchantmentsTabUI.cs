using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemEnchantmentsTabUI : MonoBehaviour
{
    [Header("Componenets")]
    [SerializeField] private TextMeshProUGUI tabLabel;
    [SerializeField] private Image tabBackground;
    [SerializeField] private GameObject enchantmentsHolder;
    [SerializeField] private GameObject slotsHolder;

    [Header("Data")]
    [SerializeField] private GameObject weaponEnchantmentPrefab;
    [SerializeField] private GameObject slotIconPrefab;

    [Header("Settings")]
    [SerializeField] private Color activeTabColor;
    [SerializeField] private Color inactiveTabColor;
    [SerializeField] private Color emptyColor;

    [Header("Debug")]
    [SerializeField] private List<WeaponEnchantmentTooltipUI> enchantmentUIs;
    [SerializeField] private List<SlotIconUI> slotIconUIs;

    private void Awake()
    {
        enchantmentUIs = new List<WeaponEnchantmentTooltipUI>();
        slotIconUIs = new List<SlotIconUI>();
    }

    public void Initialize(Weapon weapon)
    {
        for (int i = 0; i < weapon.enchantments.Count; i++)
        {
            var weaponEnchantment = Instantiate(weaponEnchantmentPrefab, enchantmentsHolder.transform).GetComponent<WeaponEnchantmentTooltipUI>();
            enchantmentUIs.Add(weaponEnchantment);

            var slotIcon = Instantiate(slotIconPrefab, slotsHolder.transform).GetComponent<SlotIconUI>();
            slotIconUIs.Add(slotIcon);

            var enchantment = weapon.enchantments[i];
            if (enchantment != null)
            {
                weaponEnchantment.Initialize(enchantment);

                Color color = ResourceMananger.instance.GetColor(enchantment.rarity);
                slotIcon.Initialize(color);
            }
            else
            {
                weaponEnchantment.Initialize(null);
                slotIcon.Initialize(emptyColor);

            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(enchantmentsHolder.GetComponent<RectTransform>());
    }

    public void Show()
    {
        tabLabel.enabled = true;
        tabBackground.color = activeTabColor;
        enchantmentsHolder.SetActive(true);
        slotsHolder.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(enchantmentsHolder.GetComponent<RectTransform>());
    }

    public void Hide()
    {
        tabLabel.enabled = false;
        tabBackground.color = inactiveTabColor;
        enchantmentsHolder.SetActive(false);
        slotsHolder.SetActive(true);
    }

    public void Uninitialize()
    {
        foreach (var tooltip in enchantmentUIs)
        {
            // Destroy
            Destroy(tooltip.gameObject);
        }
        enchantmentUIs.Clear();

        foreach (var slot in slotIconUIs)
        {
            // Destroy
            Destroy(slot.gameObject);
        }
        slotIconUIs.Clear();
    }
}
