using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropAlertUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI alertLabel;

    private ItemUI itemUI;

    public static DropAlertUI instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void Show(ItemUI itemUI)
    {
        this.itemUI = itemUI;

        alertLabel.text = $"Are you sure you want to drop your <color=yellow>{itemUI.GetItem().name}</color>?";

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Drop()
    {
        // Remove from current slot
        itemUI.GetSlotUI().StoreItem(null);

        // Destroy item
        Destroy(itemUI.gameObject);

        // Hide UI
        Hide();
    }

    public void Hide()
    {
        itemUI = null;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
