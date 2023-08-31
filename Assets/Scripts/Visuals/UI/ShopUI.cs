using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class ShopUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private GameObject shopSlotPrefab;

    [Header("Data")]
    [SerializeField] private Inventory inventory;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Start()
    {
        GameEvents.instance.onOpenShop += Open;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onOpenShop -= Open;
    }

    public void Open(Entity entity, Inventory inventory)
    {
        // Allow visibiltiy and interaction
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Open new one
        this.inventory = inventory;

        for (int i = 0; i < inventory.maxSize; i++)
        {
            // Create slot
            var shopSlot = Instantiate(shopSlotPrefab, gridLayoutGroup.transform).GetComponent<ShopSlotUI>();
            shopSlot.Initialize(entity, inventory, i);
        }
    }

    public void Close()
    {
        // Prevent visibiltiy and interaction
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Trigger event
        GameEvents.instance.TriggerOnCloseShop(inventory);
    }
}
