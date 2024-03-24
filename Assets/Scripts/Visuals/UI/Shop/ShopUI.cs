using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private GameObject shopSlotPrefab;

    [Header("Debug")]
    [SerializeField] private bool requestClose;

    private List<ShopSlotUI> shopSlots;

    public static ShopUI instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        shopSlots = new List<ShopSlotUI>();
        instance = this;
    }

    public IEnumerator Browse(Entity buyer, Inventory shopInventory)
    {
        // Allow moving of items while in menu
        GameEvents.instance.TriggerOnToggleAllowInventory(true);

        // Reset state
        requestClose = false;

        // Allow visibiltiy and interaction
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Create shop slots
        for (int i = 0; i < shopInventory.maxSize; i++)
        {
            var shopSlot = Instantiate(shopSlotPrefab, gridLayoutGroup.transform).GetComponent<ShopSlotUI>();
            shopSlot.Initialize(buyer, shopInventory, i);
            shopSlots.Add(shopSlot);
        }

        // Update layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        // Infinitely wait until player closes
        while (!requestClose)
            yield return null;

        // Clear slots
        foreach (var shopSlot in shopSlots)
        {
            Destroy(shopSlot.gameObject);
        }
        shopSlots.Clear();

        // Prevent visibiltiy and interaction
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Disallow items again
        GameEvents.instance.TriggerOnToggleAllowInventory(false);
    }

    public void Close()
    {
        requestClose = true;
    }
}
