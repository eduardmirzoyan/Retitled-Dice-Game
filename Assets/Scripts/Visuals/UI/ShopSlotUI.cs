using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopSlotUI : ItemSlotUI
{
    [SerializeField] private Entity buyer;
    [SerializeField] private Inventory inventory;
    [SerializeField] private int index;

    public void Initialize(Entity buyer, Inventory inventory, int index)
    {
        this.buyer = buyer;
        this.inventory = inventory;
        this.index = index;

        if (inventory[index] != null)
        {
            // Create item object
            var itemUI = Instantiate(itemUIPrefab, transform).GetComponent<ItemUI>();
            itemUI.Initialize(inventory[index], this);

            // Update afterimage
            afterImageIcon.sprite = inventory[index].sprite;
            afterImageIcon.enabled = true;

            // Save item
            this.itemUI = itemUI;

            // Check
            UpdateEligibility(buyer, 0);
        }

        // Update name
        gameObject.name = "Shop Slot " + index;

        GameEvents.instance.onCloseShop += Uninitialize;
        GameEvents.instance.onEntityGoldChange += UpdateEligibility;
    }

    private void UpdateEligibility(Entity entity, int gain)
    {
        if (this.buyer == entity && this.itemUI != null)
        {
            // Check if item can be bought by buyer
            if (buyer.gold >= inventory[index].value)
            {
                itemUI.PreventRemove(false);
            }
            else
            {
                itemUI.PreventRemove(true);
            }
        }
    }

    private void OnDestroy()
    {
        GameEvents.instance.onCloseShop -= Uninitialize;
        GameEvents.instance.onEntityGoldChange -= UpdateEligibility;
    }

    private void Uninitialize(Inventory inventory)
    {
        if (this.inventory == inventory)
        {
            Destroy(gameObject);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        // Do nothing.
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // Do nothing.
    }

    public override void OnDrop(PointerEventData eventData)
    {
        // Do nothing.
    }

    public override void StoreItem(ItemUI itemUI)
    {
        if (itemUI != null) throw new System.Exception("PROBLEM: A ITEM WAS INSERTED INTO SHOP SLOT?");

        // Update data
        afterImageIcon.enabled = false;
        this.itemUI = null;

        // Remove gold from buyer
        buyer.gold -= inventory[index].value;

        // Trigger event
        GameEvents.instance.TriggerOnGoldChange(buyer, -inventory[index].value);
    }

    // private void UpdateSlot(ItemUI newItemUI)
    // {
    //     // If an item already exists, swap
    //     if (this.itemUI != null)
    //     {
    //         // Debugging
    //         print("An item exists in this slot, doing a swap :)");

    //         // Set the old item to where the new one was
    //         this.itemUI.ResetTo(newItemUI.GetParent());
    //     }

    //     // Set any previous slots to null;
    //     if (newItemUI.GetItemSlotUI() != null)
    //     {
    //         // Add item to slot, where this.itemUI could be null in which case you are un-equipping
    //         newItemUI.GetItemSlotUI().StoreItem(this.itemUI);
    //     }

    //     // Store new item into this slot
    //     StoreItem(newItemUI);
    // }

    // public override void StoreItem(ItemUI itemUI)
    // {
    //     // Actual Item
    //     if (itemUI != null)
    //     {
    //         // Debugging
    //         // print("Item " + itemUI.name + " has inserted into the slot: " + name);

    //         var item = itemUI.GetItem();

    //         // Change slot
    //         itemUI.SetParent(gameObject.transform);
    //         itemUI.SetItemSlot(this);

    //         // Update afterimage
    //         afterImageIcon.sprite = item.sprite;
    //         afterImageIcon.enabled = true;

    //         // Update inventory
    //         inventory.SetItem(item, index);
    //     }
    //     else
    //     {
    //         // Debugging
    //         // print("Item was removed from slot: " + name);

    //         // Disable afterimage
    //         afterImageIcon.enabled = false;

    //         // Update inventory
    //         inventory.SetItem(null, index);
    //     }

    //     this.itemUI = itemUI;
    // }

}
