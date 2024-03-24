using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnityEnchantmentUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Header("Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Outline rarityOutline;
    [SerializeField] private Shadow hoverShadow;
    [SerializeField] private Outline hoverOutline;

    [Header("Debug")]
    [SerializeField] private EntityEnchantment entityEnchantment;

    public void Initialize(EntityEnchantment entityEnchantment)
    {
        this.entityEnchantment = entityEnchantment;
        iconImage.sprite = entityEnchantment.icon;
        iconImage.enabled = true;
        rarityOutline.effectColor = ResourceMananger.instance.GetColor(entityEnchantment.rarity);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverShadow.enabled = true;
        hoverOutline.enabled = true;

        // Display tooltip from bottom-left
        var corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        EntityEnchantmentTooltipUI.instance.Show(entityEnchantment, corners[0]);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameManager.instance.PlayerGainEnchantment(entityEnchantment);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverShadow.enabled = false;
        hoverOutline.enabled = false;
        EntityEnchantmentTooltipUI.instance.Hide();
    }
}
