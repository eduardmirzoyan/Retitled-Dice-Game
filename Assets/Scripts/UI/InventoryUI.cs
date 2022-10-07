using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private List<ItemSlotUI> itemSlots;
    [SerializeField] private Image lockImage;

    [Header("Header")]
    [SerializeField] private Player player;
    [SerializeField] private Room room;

    private bool isLocked;
    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += Initialize;
        GameEvents.instance.onItemInsert += SetItem;
        GameEvents.instance.onRemoveEntity += CheckUnlock;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= Initialize;
        GameEvents.instance.onItemInsert -= SetItem;
        GameEvents.instance.onRemoveEntity -= CheckUnlock;
    }

    private void LockUI()
    {
        isLocked = true;
        lockImage.enabled = true;
    }

    private void UnlockUI()
    {
        isLocked = false;
        lockImage.enabled = false;
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

        // Set locked state
        isLocked = room.enemies.Count > 0;

        // Check how many enemies are there
        if (isLocked)
        {
            Lock();
        }
        else
        {
            Unlock();
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

    private void CheckUnlock(Entity entity)
    {
        // When an enemy dies, check if there are 0 left
        if (isLocked && room.enemies.Count == 0)
        {
            // Unlock
            Unlock();
        }
        else if (!isLocked && room.enemies.Count > 0)
        {
            // Lock
            Lock();
        }
    }

    private void Lock()
    {
        // Show visuals
        lockImage.enabled = true;

        // Lock each itemslot
        foreach (var itemSlot in itemSlots)
        {
            // Lock slot
            itemSlot.DisableRemove();
        } 
    }

    private void Unlock()
    {
        // Show visuals
        lockImage.enabled = false;

        // Unlock each itemslot
        foreach (var itemSlot in itemSlots)
        {
            // Lock slot
            itemSlot.EnableRemove();
        }
    }
}
