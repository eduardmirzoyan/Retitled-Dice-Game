using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI instance;

    [Header("Displaying Components")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private GameObject actionUIPrefab;
    [SerializeField] private LayoutGroup actionsLayoutGroup;
    [SerializeField] private TextMeshProUGUI valueText;

    private List<ActionUI> actionUIs;

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
        actionUIs = new List<ActionUI>();
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

    public void Show(Item item, bool showForSale = false)
    {
        // Display window
        canvasGroup.alpha = 1f;

        // Update information
        itemName.text = item.name;
        itemDescription.text = item.description;
        
        // Update selling info, IMPROVE LATER?
        valueText.text = "" + item.value;
        valueText.transform.parent.gameObject.SetActive(showForSale);
        

        // If the item is an equipment item
        if (item is Weapon)
        {
            // Cast
            var weapon = (Weapon)item;

            // Display all its skills
            foreach (var action in weapon.actions)
            {
                // Spawn visuals of actions
                var actionUI = Instantiate(actionUIPrefab, actionsLayoutGroup.transform).GetComponent<ActionUI>();
                // Initialize as display
                actionUI.Initialize(action, true);

                // Save
                actionUIs.Add(actionUI);
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
        foreach (var ui in actionUIs)
        {
            // Uninit
            ui.Uninitialize();
            // Destroy
            Destroy(ui.gameObject);
        }
        actionUIs.Clear();

        // Then disable window
        canvasGroup.alpha = 0f;
    }

}
