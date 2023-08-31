using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI instance;

    [Header("Static Data")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private GameObject actionTooltipPrefab;
    [SerializeField] private LayoutGroup actionsLayoutGroup;
    [SerializeField] private TextMeshProUGUI valueText;

    private List<ActionTooltipUI> actionTooltips;

    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        canvasGroup = GetComponentInChildren<CanvasGroup>();

        // lockImage.enabled = false;
        actionTooltips = new List<ActionTooltipUI>();
    }

    private void Update()
    {
        FollowMouse();
    }

    private void FollowMouse()
    {
        // Update position
        Vector2 position = Input.mousePosition;
        Vector2 adjustedPosition = Camera.main.ScreenToWorldPoint(position);
        transform.position = adjustedPosition;

        UpdatePivot(position);
    }

    private void UpdatePivot(Vector2 mousePosition)
    {
        var width = rectTransform.rect.width;
        var height = rectTransform.rect.height;

        int pivotX = 0;
        int pivotY = 0;

        // Check if window goes off-screen on x-axis
        // If so, 
        if (mousePosition.x + width > Screen.width)
        {
            // Change pivot to right of window
            pivotX = 1;
        }

        // Check if window goes off-screen on y-axis
        // If so, flip vertically
        if (mousePosition.y + height > Screen.height)
        {
            // Change pivot to top of window
            pivotY = 1;
        }

        // Set updated pivot
        rectTransform.pivot = new Vector2(pivotX, pivotY);
    }

    public void Show(Item item, bool showPrice = false)
    {
        // Display window
        canvasGroup.alpha = 1f;

        // Update information
        itemName.text = item.name;
        itemDescription.text = item.description;

        // Update selling info
        if (showPrice)
        {
            valueText.gameObject.SetActive(true);
            valueText.fontSize = 48f;
            valueText.text = "Price: <sprite name=\"Gold\">" + item.GetValue();
        }
        else if (item is Consumable)
        {
            valueText.gameObject.SetActive(true);
            valueText.fontSize = 36f;
            valueText.text = "[Right Click] to use";
        }
        else
        {
            valueText.gameObject.SetActive(false);
        }

        // If the item is an equipment item
        if (item is Weapon)
        {
            // Cast
            var weapon = (Weapon)item;

            // Display all its skills
            foreach (var action in weapon.actions)
            {
                // Spawn visuals of actions
                var actionTooltip = Instantiate(actionTooltipPrefab, actionsLayoutGroup.transform).GetComponent<ActionTooltipUI>();
                // Initialize as display
                actionTooltip.Initialize(action);

                // Save
                actionTooltips.Add(actionTooltip);
            }
        }

        // Update Canvas
        LayoutRebuilder.ForceRebuildLayoutImmediate(actionsLayoutGroup.GetComponent<RectTransform>());

        // Update Canvas
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void Hide()
    {
        // Destroy all the ui
        foreach (var tooltip in actionTooltips)
        {
            // Destroy
            Destroy(tooltip.gameObject);
        }
        actionTooltips.Clear();

        // Then disable window
        canvasGroup.alpha = 0f;
    }

}
