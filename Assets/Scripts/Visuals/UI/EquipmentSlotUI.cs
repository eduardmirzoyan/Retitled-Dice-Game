using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : ItemSlotUI, IDropHandler
{
    [SerializeField] private Player player;
    [SerializeField] private int index;
    [SerializeField] private bool inCombat;

    private void OnDestroy()
    {
        GameEvents.instance.onCombatEnter -= EnterCombat;
        GameEvents.instance.onCombatExit -= ExitCombat;
    }

    public void Initialize(Player player, int index)
    {
        this.player = player;
        this.index = index;

        if (player.weapons[index] != null)
        {
            // Create item object
            var itemUI = Instantiate(itemUIPrefab, transform).GetComponent<ItemUI>();
            itemUI.Initialize(player.weapons[index], this);

            // Update afterimage
            afterImageIcon.sprite = player.weapons[index].sprite;
            afterImageIcon.enabled = true;

            // Save item
            this.itemUI = itemUI;
        }

        // Update name
        if (index == 0)
        {
            gameObject.name = "Mainhand Slot";
        }
        else
        {
            gameObject.name = "Offhand Slot";
        }

        inCombat = false;

        GameEvents.instance.onCombatEnter += EnterCombat;
        GameEvents.instance.onCombatExit += ExitCombat;
    }

    private void EnterCombat()
    {
        inCombat = true;
    }

    private void ExitCombat()
    {
        inCombat = false;
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
            if (newItemUI.GetItem() is not Weapon) return;
            if (inCombat && !DataManager.instance.hasEquipmentPrompted)
            {
                // Open prompt
                EquipmentAlertUI.instance.Show();
                DataManager.instance.hasEquipmentPrompted = true;
                return;
            }

            // Now we all good, do insertion...
            UpdateSlot(newItemUI);
        }
    }

    private void UpdateSlot(ItemUI newItemUI)
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

            // Update weapon
            player.EquipWeapon((Weapon)item, index);

            // If we are in combat, end turn
            if (inCombat)
            {
                GameManager.instance.EndTurnNow();
            }
        }
        else
        {
            // Debugging
            //print("Item was removed from slot: " + name);

            // Disable afterimage
            afterImageIcon.enabled = false;

            // Update weapon
            player.EquipWeapon(null, index);
        }

        this.itemUI = itemUI;
    }
}
