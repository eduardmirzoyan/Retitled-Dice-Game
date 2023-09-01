using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class ItemSlotUI : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    [SerializeField] protected Image highlightImage;
    [SerializeField] protected Image afterImageIcon;
    [SerializeField] protected Color defaultColor;
    [SerializeField] protected Color highlightColor;

    [Header("Data")]
    [SerializeField] protected ItemUI itemUI;
    [SerializeField] protected GameObject itemUIPrefab;

    [Header("Settings")]
    [SerializeField] public bool preventInsert = false;

    public abstract void OnPointerEnter(PointerEventData eventData);

    public abstract void OnPointerExit(PointerEventData eventData);

    public abstract void OnDrop(PointerEventData eventData);

    public abstract void StoreItem(ItemUI itemUI);

    public bool PreventingInsert() => preventInsert;

}
