using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionTooltipUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private TextMeshProUGUI actionTypeText;
    [SerializeField] private TextMeshProUGUI actionRangeText;
    [SerializeField] private TextMeshProUGUI actionDescriptionText;
    [SerializeField] private TextMeshProUGUI actionModifiersText;
    [SerializeField] private TextMeshProUGUI actionSourceText;
    [SerializeField] private GameObject modifierSeperator;
    [SerializeField] private TextMeshProUGUI hotkeyText;

    public static ActionTooltipUI instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void Show(Action action, string hotkey, Vector3 position)
    {
        // Set textual info
        actionNameText.text = action.GetDynamicName();
        actionTypeText.text = $"{action.actionType} Action";
        actionTypeText.color = action.color;

        actionRangeText.text = $"Range: <color=yellow>{action.die.minValue} - {action.die.maxValue}</color>";

        actionDescriptionText.text = action.GetActiveDescription();

        hotkeyText.text = $"[{hotkey}]";

        actionModifiersText.text = "";
        foreach (var modifier in action.modifiers)
        {
            actionModifiersText.text += $"{modifier}\n";
        }
        modifierSeperator.SetActive(action.modifiers.Count > 0);

        if (action.weapon != null)
        {
            actionSourceText.text = $"Source: {action.weapon.name}";
        }
        else
        {
            actionSourceText.text = $"Source: Innate";
        }

        // Show
        canvasGroup.alpha = 1f;

        // Relocate
        transform.position = position;
        UpdatePivot(position);

        // Update Canvas
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }

    public void Hide()
    {
        // Show
        canvasGroup.alpha = 0f;
    }

    private void UpdatePivot(Vector2 worldPosition)
    {
        var rectTransform = GetComponent<RectTransform>();
        var width = rectTransform.rect.width;
        var height = rectTransform.rect.height;

        int pivotX = 0;
        int pivotY = 0;

        var screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Check if window goes off-screen on x-axis
        // If so, 
        if (screenPosition.x + width > Screen.width)
        {
            // Change pivot to right of window
            pivotX = 1;
        }

        // Check if window goes off-screen on y-axis
        // If so, flip vertically
        if (screenPosition.y + height > Screen.height)
        {
            // Change pivot to top of window
            pivotY = 1;
        }

        // Set updated pivot
        rectTransform.pivot = new Vector2(pivotX, pivotY);
    }

    private void OnDrawGizmosSelected()
    {
        var rectTransform = GetComponent<RectTransform>();
        Gizmos.DrawWireSphere(rectTransform.pivot, 0.25f);
    }
}
