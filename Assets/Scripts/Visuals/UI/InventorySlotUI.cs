using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotUI : ItemSlotUI
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private int index;

    public void Initialize(Inventory inventory, int index)
    {
        this.inventory = inventory;
        this.index = index;

        if (inventory[index] != null)
        {
            // Create item object
            var itemUI = Instantiate(itemUIPrefab, transform).GetComponent<ItemUI>();
            itemUI.Initialize(inventory[index], this);

            // Update afterimage
            afterImageIcon.sprite = itemUI.GetItem().sprite;
            afterImageIcon.enabled = true;

            // Save item
            this.itemUI = itemUI;
        }

        // Update name
        gameObject.name = "Inventory Slot " + index;
    }

    public override void OnDrop(PointerEventData eventData)
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

            // Now we all good, do insertion...
            UpdateSlot(newItemUI);
        }
    }

    protected void UpdateSlot(ItemUI newItemUI)
    {
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

    public override void StoreItem(ItemUI itemUI)
    {
        // Actual Item
        if (itemUI != null)
        {
            // Debugging
            //print("Item " + itemUI.name + " has inserted into the slot: " + name);

            var item = itemUI.GetItem();

            // Change slot
            itemUI.SetParent(gameObject.transform);
            itemUI.SetItemSlot(this);

            // Update afterimage
            afterImageIcon.sprite = item.sprite;
            afterImageIcon.enabled = true;

            // Update inventory
            inventory.SetItem(item, index);
        }
        else
        {
            // Debugging
            //print("Item was removed from slot: " + name);

            // Disable afterimage
            afterImageIcon.enabled = false;

            // Update inventory
            inventory.SetItem(null, index);
        }

        this.itemUI = itemUI;
    }
}
