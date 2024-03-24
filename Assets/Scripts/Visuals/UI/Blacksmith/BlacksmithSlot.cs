using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BlacksmithSlot : ItemSlotUI, IPointerClickHandler
{
    public void Initialize(Weapon weapon)
    {
        if (weapon != null)
        {
            // Create item object
            var itemUI = Instantiate(itemUIPrefab, transform).GetComponent<ItemUI>();
            itemUI.Initialize(weapon, this);

            // Update afterimage
            afterImageIcon.sprite = weapon.sprite;
            afterImageIcon.enabled = true;

            // Save item
            this.itemUI = itemUI;
        }

        // Update name
        gameObject.name = "Blacksmith Slot";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemUI != null && eventData.button == PointerEventData.InputButton.Right)
        {
            // Remove stored item
            StoreItem(null);

            highlightImage.color = defaultColor;
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (preventInsert) return;

        if (itemUI != null || (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out ItemUI _)))
        {
            highlightImage.color = highlightColor;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (preventInsert) return;

        if (itemUI != null || (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out ItemUI _)))
        {
            highlightImage.color = defaultColor;
        }
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
            if (itemUI != null && newItemUI.GetSlotUI().preventInsert) return;
            if (newItemUI.GetItem() is not Weapon) return;

            StoreItem(newItemUI);
        }
    }

    public override void StoreItem(ItemUI newItemUI)
    {
        // Do nothing if the same
        if (newItemUI == itemUI) return;

        // If old item exists
        if (itemUI != null)
        {
            // Debugging
            // print("Item " + itemUI.name + " was removed from slot: " + name);

            // Disable afterimage
            afterImageIcon.enabled = false;

            // Blacksmith logic here
            GameEvents.instance.TriggerOnRemoveBlacksmith(null);
        }

        // If new item exists
        if (newItemUI != null)
        {
            // Debugging
            // print("Item " + newItemUI.name + " was inserted into slot: " + name);

            var item = newItemUI.GetItem();

            // Update afterimage
            afterImageIcon.sprite = item.sprite;
            afterImageIcon.enabled = true;

            // Blacksmith logic here
            GameEvents.instance.TriggerOnInsertBlacksmith((Weapon)item);
        }


        this.itemUI = newItemUI;
    }
}
