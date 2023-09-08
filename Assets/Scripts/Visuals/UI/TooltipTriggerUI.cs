using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipTriggerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string header;

    [TextArea(10, 20)]
    [SerializeField] private string description;

    public void SetTooltip(string header, string description)
    {
        this.header = header;
        this.description = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TextTooltipUI.instance.Show(header, description);
        if (TryGetComponent(out Outline outline))
        {
            outline.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TextTooltipUI.instance.Hide();
        if (TryGetComponent(out Outline outline))
        {
            outline.enabled = false;
        }
    }
}
