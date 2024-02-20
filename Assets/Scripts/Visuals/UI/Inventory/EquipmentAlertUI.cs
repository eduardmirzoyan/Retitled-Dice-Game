using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentAlertUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    public static EquipmentAlertUI instance;
    private void Awake()
    {
        // Singleton Logic
        if (EquipmentAlertUI.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
