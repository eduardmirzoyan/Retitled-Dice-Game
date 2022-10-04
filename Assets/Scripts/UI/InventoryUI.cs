using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private List<ItemSlotUI> itemSlots;

    [Header("Header")]
    [SerializeField] private Player player;

    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += Initialize;
        GameEvents.instance.onItemInsert += AddItem;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= Initialize;
        GameEvents.instance.onItemInsert -= AddItem;
    }

    private void Initialize(Room room)
    {
        this.player = room.player;

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

    private void AddItem(ItemUI itemUI, ItemSlotUI itemSlotUI)
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
