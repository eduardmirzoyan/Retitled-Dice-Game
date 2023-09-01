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

        // Don't allow insertions
        preventInsert = true;

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
}
