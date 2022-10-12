using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Transform gridTransform;

    private List<ItemSlotUI> itemSlotUIs;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        itemSlotUIs = new List<ItemSlotUI>();
    }

    private void Start()
    {
        GameEvents.instance.onOpenShop += Open;
    }

    public void Initialize(Inventory inventory)
    {
        // Should fill this UI with items based on inventory
        foreach (var item in inventory.GetItems())
        {
            if (item != null)
            {
                // Create slot
                var itemSlot = Instantiate(itemSlotPrefab, gridTransform).GetComponent<ItemSlotUI>();
                // Set item
                itemSlot.CreateItem(item);
                // Save
                itemSlotUIs.Add(itemSlot);
            }
        }
    }

    public void Open(Inventory inventory)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Initalize
        Initialize(inventory);
    }

    public void Close()
    {
        // Delete items
        foreach (var itemSlot in itemSlotUIs)
        {
            Destroy(itemSlot.gameObject);
        }
        itemSlotUIs.Clear();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
