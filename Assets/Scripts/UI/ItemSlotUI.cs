using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    [SerializeField] private Image highlightImage;

    [Header("Data")]
    [SerializeField] private ItemUI itemUI;
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private bool isActive = true;

    [Header("Settings")]
    [SerializeField] private bool weaponsOnly = false;

    public void CreateItem(Item item)
    {
        // Create item object
        var itemUI = Instantiate(itemUIPrefab, transform).GetComponent<ItemUI>();
        itemUI.Initialize(item, this);

        // Save item
        this.itemUI = itemUI;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Make sure slot is active
        if (!isActive) return;

        // Make sure there already isn't an item and it is an itemUI
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out ItemUI newItemUI))
        {
            // Remove highlight
            var color = highlightImage.color;
            color.a = 0f;
            highlightImage.color = color;

            // Make sure that the same item isn't added to the same slot
            if (newItemUI == itemUI) return;

            // Check settings
            if (weaponsOnly && itemUI.GetItem() is not Weapon) return;

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
        // Make sure slot is active
        if (!isActive) return;

        if (eventData.pointerDrag != null && itemUI == null && eventData.pointerDrag.TryGetComponent(out ItemUI newItemUI))
        {
            if (highlightImage != null)
            {
                var color = highlightImage.color;
                color.a = 0.35f;
                highlightImage.color = color;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Make sure slot is active
        if (!isActive) return;

        if (eventData.pointerDrag != null && itemUI == null && eventData.pointerDrag.TryGetComponent(out ItemUI newItemUI))
        {
            if (highlightImage != null)
            {
                var color = highlightImage.color;
                color.a = 0f;
                highlightImage.color = color;
            }
        }
    }
}
