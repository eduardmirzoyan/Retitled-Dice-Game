using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    [SerializeField] private Image highlightImage;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Image afterImageIcon;
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
    [SerializeField] private bool mustPurchase = false;

    public void CreateItem(Item item)
    {
        // Create item object
        var itemUI = Instantiate(itemUIPrefab, transform).GetComponent<ItemUI>();
        itemUI.Initialize(item, this);

        // Update afterimage
        afterImageIcon.sprite = itemUI.GetItem().sprite;
        afterImageIcon.enabled = true;

        // Save item
        this.itemUI = itemUI;
    }

    public void SetShopSlot(bool state)
    {
        // Set this slot to a shop slot...

        // Must pay price to remove items
        mustPurchase = state;

        // Prevent adding items
        preventInsert = state;
    }

    public void Lock()
    {
        preventRemove = true;
        preventInsert = true;

        // Show lock icon
        lockIcon.enabled = true;

        // Set item inside to not interactable
        if (itemUI != null)
            itemUI.SetInteractable(false);
    }

    public void Unlock()
    {
        preventRemove = false;
        preventInsert = false;

        // Hide lock icon
        lockIcon.enabled = false;

        // Set item inside to not interactable
        if (itemUI != null)
            itemUI.SetInteractable(true);
    }

    public bool PreventRemove()
    {
        return preventRemove;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Remove highlight
        highlightImage.color = defaultColor;

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
            if (newItemUI.GetItemSlotUI() != null && newItemUI.GetItemSlotUI().mustPurchase && !Buy(newItemUI.GetItem().GetValue())) return;

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
        // Actual Item
        if (itemUI != null)
        {
            // Debugging
            print("Item " + itemUI.name + " has inserted into the slot: " + name);

            // Change slot
            itemUI.SetParent(gameObject.transform);
            itemUI.SetItemSlot(this);

            // Update afterimage
            afterImageIcon.sprite = itemUI.GetItem().sprite;
            afterImageIcon.enabled = true;

            // Trigger event
            GameEvents.instance.TriggerOnItemInsert(itemUI, this);
        }
        else
        {
            // Debugging
            print("Item was removed from slot: " + name);

            // Disable afterimage
            afterImageIcon.enabled = false;

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

    private bool Buy(int cost)
    {
        if (DataManager.instance.GetPlayer().gold >= cost)
        {
            DataManager.instance.GetPlayer().AddGold(-cost);
            return true;
        }

        // If can't buy return false
        return false;
    }

    public bool MustBuy()
    {
        return mustPurchase;
    }


}
