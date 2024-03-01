using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EntityEnchantmentSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Outline outline;

    private EntityEnchantment enchantment;

    public void Initialize(EntityEnchantment enchantment)
    {
        this.enchantment = enchantment;

        iconImage.sprite = enchantment.icon;
        iconImage.enabled = true;
        outline.effectColor = ResourceMananger.instance.GetColor(enchantment.rarity);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Display tooltip from bottom-left
        var corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        EntityEnchantmentTooltipUI.instance.Show(enchantment, corners[0]);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EntityEnchantmentTooltipUI.instance.Hide();
    }
}
