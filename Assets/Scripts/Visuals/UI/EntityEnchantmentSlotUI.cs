using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityEnchantmentSlotUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Outline outline;
    [SerializeField] private TooltipTriggerUI tooltipTrigger;

    public void Initialize(EntityEnchantment enchantment)
    {
        iconImage.sprite = enchantment.icon;
        iconImage.enabled = true;
        outline.effectColor = ResourceMananger.instance.GetColor(enchantment.rarity);
        tooltipTrigger.SetTooltip(enchantment.name, enchantment.description);
    }
}
