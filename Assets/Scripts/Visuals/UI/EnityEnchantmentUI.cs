using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnityEnchantmentUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Header("Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Outline outline;
    [SerializeField] private Shadow shadow;

    [Header("Debug")]
    [SerializeField] private EntityEnchantment entityEnchantment;

    public void Initialize(EntityEnchantment entityEnchantment)
    {
        this.entityEnchantment = entityEnchantment;
        iconImage.sprite = entityEnchantment.icon;
        iconImage.enabled = true;
        outline.effectColor = ResourceMananger.instance.GetColor(entityEnchantment.rarity);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        shadow.enabled = true;
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
        shadow.enabled = false;
    }
}
