using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private List<ItemSlotUI> itemSlots;

    [Header("Header")]
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Inventory inventory;

    private bool isLocked;
    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += Initialize;
        GameEvents.instance.onItemInsert += SetItem;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= Initialize;
        GameEvents.instance.onItemInsert -= SetItem;
    }

    private void Initialize(Room room)
    {
        this.inventory = room.player.inventory;

        // Intialize slots
        itemSlots = new List<ItemSlotUI>();

        // Fill item slots with entity's inventory
        for (int i = 0; i < inventory.maxSize; i++)
        {
            // Create slot in grid
            var itemSlot = Instantiate(itemSlotPrefab, gridLayoutGroup.transform).GetComponent<ItemSlotUI>();

            // Save ref
            itemSlots.Add(itemSlot);

            // Set item
            if (inventory[i] != null)
            {
                // Create item here
                itemSlots[i].CreateItem(inventory[i]);
            }
        }
    }

    private void SetItem(ItemUI itemUI, ItemSlotUI itemSlotUI)
    {
        // If inventory is not set, then dip
        if (inventory == null) return;

        // Check if item was added to any of this inventory's slots
        for (int i = 0; i < itemSlots.Count; i++)
        {
            // If item was inserted into here
            if (itemSlots[i] == itemSlotUI)
            {
                // Set item
                if (itemUI != null) inventory.SetItem(itemUI.GetItem(), i);
                else inventory.SetItem(null, i);
            }
        }
    }
}
