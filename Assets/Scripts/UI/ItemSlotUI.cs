using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    [SerializeField] private Image highlightImage;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color disabledColor;

    [Header("Data")]
    [SerializeField] private ItemUI itemUI;
    [SerializeField] private GameObject itemUIPrefab;

    [Header("Settings")]
    [SerializeField] private bool preventInsert = false;
    [SerializeField] private bool preventRemove = false;
    [SerializeField] private bool weaponsOnly = false;    
    [SerializeField] private bool mustBuy = false;

    public void CreateItem(Item item)
    {
        // Create item object
        var itemUI = Instantiate(itemUIPrefab, transform).GetComponent<ItemUI>();
        itemUI.Initialize(item, this);

        // Save item
        this.itemUI = itemUI;
    }

    public void DisableRemove()
    {
        preventRemove = true;

        // Change color
        highlightImage.color = disabledColor;

        // Set item inside to not interactable
        if (itemUI != null)
            itemUI.SetInteractable(false);
    }

    public void EnableRemove()
    {
        preventRemove = false;

        // Change color
        highlightImage.color = defaultColor;

        // Set item inside to not interactable
        if (itemUI != null)
            itemUI.SetInteractable(true);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Make sure there already isn't an item and it is an itemUI
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out ItemUI newItemUI))
        {
            // Make sure that the same item isn't added to the same slot
            if (newItemUI == itemUI) return;

            // Check restrictions
            if (preventInsert) return;

            // Check restrictions
            if (weaponsOnly && newItemUI.GetItem() is not Weapon) return;

            // Attempt to buy
            if (newItemUI.GetItemSlotUI() != null && newItemUI.GetItemSlotUI().mustBuy && !Buy(newItemUI.GetItem().value)) return;

            // Remove highlight
            highlightImage.color = defaultColor;

            // Now we all good, do insertion...

            // If an item already exists, swap
            if (this.itemUI != null)
            {
                // Debugging
                print("An item exists in this slot, doing a swap :)");

                // Set the old item to where the new one was
                this.itemUI.ResetTo(newItemUI.GetParent());
            }

            // Set any previous slots to null;
            if (newItemUI.GetItemSlotUI() != null)
            {
                // Add item to slot, where this.itemUI could be null in which case you are un-equipping
                newItemUI.GetItemSlotUI().StoreItem(this.itemUI);
            }

            // Store new item into this slot
            StoreItem(newItemUI);
        }
    }

    public void StoreItem(ItemUI itemUI)
    {
        if (itemUI != null)
        {
            // Debugging
            print("Item " + itemUI.name + " has inserted into the slot: " + name);
            itemUI.SetParent(gameObject.transform);
            itemUI.SetItemSlot(this);

            // Trigger event
            GameEvents.instance.TriggerOnItemInsert(itemUI, this);
        }
        else
        {
            // Debugging
            print("Item was removed from slot: " + name);

            // Trigger event
            GameEvents.instance.TriggerOnItemInsert(null, this);
        }

        this.itemUI = itemUI;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (preventInsert) return;

        if (eventData.pointerDrag != null && itemUI == null && eventData.pointerDrag.TryGetComponent(out ItemUI newItemUI))
        {
            highlightImage.color = highlightColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (preventInsert) return;

        if (eventData.pointerDrag != null && itemUI == null && eventData.pointerDrag.TryGetComponent(out ItemUI newItemUI))
        {
            highlightImage.color = defaultColor;
        }
    }

    private bool Buy(int cost) {
        if (DataManager.instance.GetPlayer().gold >= cost) {
            DataManager.instance.GetPlayer().AddGold(-cost);
            return true;
        }
        
        // If can't buy return false
        return false;   
    }

    private bool PassesRestrictions() {


        return true;
    }

    public bool MustBuy() {
        return mustBuy;
    }

    public bool PreventRemove() {
        return preventRemove;
    }
}
