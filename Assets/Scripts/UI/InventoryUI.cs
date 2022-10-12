using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private List<ItemSlotUI> itemSlots;

    [Header("Header")]
    [SerializeField] private Player player;
    [SerializeField] private Room room;

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
        this.player = room.player;
        this.room = room;

        // Fill item slots with player's inventory
        for (int i = 0; i < player.inventory.maxSize; i++)
        {
            if (player.inventory[i] != null)
            {
                // Create item here
                itemSlots[i].CreateItem(player.inventory[i]);
            }
        }
    }

    private void SetItem(ItemUI itemUI, ItemSlotUI itemSlotUI)
    {
        // Check if item was added to any of this inventory's slots
        for (int i = 0; i < itemSlots.Count; i++)
        {
            // If item was inserted into here
            if (itemSlots[i] == itemSlotUI)
            {
                // Set item
                if (itemUI != null) player.inventory.SetItem(itemUI.GetItem(), i);
                else player.inventory.SetItem(null, i);
            }
        }
    }
}
