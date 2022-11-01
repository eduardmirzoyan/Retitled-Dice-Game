using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    [Header("Data")]
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private List<ItemSlotUI> itemSlots;
    [SerializeField] private Inventory inventory;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        itemSlots = new List<ItemSlotUI>();
    }

    private void Start()
    {
        GameEvents.instance.onOpenShop += Open;
    }

    public void Initialize(Inventory inventory)
    {
        this.inventory = inventory;
        
        // Should fill this UI with items based on inventory
        foreach (var item in inventory.GetItems())
        {
            // Create slot
            var itemSlot = Instantiate(itemSlotPrefab, gridLayoutGroup.transform).GetComponent<ItemSlotUI>();

            // Set slot as shop
            itemSlot.SetShopSlot(true);

            if (item != null)
            {
                // Set item
                itemSlot.CreateItem(item);
            }

            // Save
            itemSlots.Add(itemSlot);
        }
    }

    public void Open(Inventory inventory)
    {
        // Allow visibiltiy and interaction
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // If this inventory is different than the one being open
        if (this.inventory != inventory) {
            // Clear this inventory
            ClearInventory();

            // Open new one
            Initialize(inventory);
        }
        
        // Sub to events
        GameEvents.instance.onItemInsert += SetItem;
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
                if (itemUI != null) inventory.SetItem(itemUI.GetItem(), i);
                else inventory.SetItem(null, i);
            }
        }
    }

    private void ClearInventory() {
        // If inventory is already clear, then dip
        if (itemSlots.Count == 0) return;

        // Delete items
        foreach (var itemSlot in itemSlots)
        {
            Destroy(itemSlot.gameObject);
        }
        itemSlots.Clear();

        // Remove inventory
        inventory = null;
    }

    public void Close()
    {
        // Prevent visibiltiy and interaction
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Unsub
        GameEvents.instance.onItemInsert -= SetItem;
    }
}
