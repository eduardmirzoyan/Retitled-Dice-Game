using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltipUI : MonoBehaviour
{
    private enum TabMode { Actions, Enchantments }
    private enum VisibilityState { Visible, Transitioning, Invisible }

    [Header("Static Data")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI auxiliaryText;
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private ItemActionsTabUI actionsTabUI;
    [SerializeField] private ItemEnchantmentsTabUI enchantmentsTabUI;

    [Header("Tab Components")]
    [SerializeField] private GameObject containerObject;
    [SerializeField] private TabMode tabMode;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.LeftShift;
    [SerializeField] private float delay;

    [Header("Debug")]
    [SerializeField] private VisibilityState state;
    // [SerializeField] private bool isVisible;
    // [SerializeField] private bool isTransitioning;

    public static ItemTooltipUI instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        tabMode = TabMode.Actions;
        state = VisibilityState.Invisible;

        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Update()
    {
        if (state == VisibilityState.Visible)
        {
            if (Input.GetKey(toggleKey))
            {
                if (tabMode == TabMode.Actions)
                {
                    actionsTabUI.Hide();
                    enchantmentsTabUI.Show();
                    instructionsText.text = "Release [Shift] to switch tabs";

                    LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                    UpdatePivot();

                    tabMode = TabMode.Enchantments;
                }
            }
            else if (tabMode == TabMode.Enchantments)
            {
                actionsTabUI.Show();
                enchantmentsTabUI.Hide();
                instructionsText.text = "Hold [Shift] to switch tabs";

                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                UpdatePivot();

                tabMode = TabMode.Actions;
            }
        }
    }

    public void Show(Item item, Vector3 position, bool showPrice = false)
    {
        if (state == VisibilityState.Visible) return;

        if (item is Weapon)
        {
            Weapon weapon = item as Weapon;
            itemNameText.text = weapon.name;
            itemTypeText.text = "Weapon";
            itemDescriptionText.text = $"Base Damage: <color=yellow>{weapon.baseDamage}</color>";
            auxiliaryText.text = "";

            actionsTabUI.Initialize(weapon);
            enchantmentsTabUI.Initialize(weapon);

            containerObject.SetActive(true);
            if (Input.GetKey(toggleKey))
            {
                actionsTabUI.Hide();
                enchantmentsTabUI.Show();
            }
            else
            {
                actionsTabUI.Show();
                enchantmentsTabUI.Hide();
            }
        }
        else if (item is Consumable)
        {
            itemNameText.text = item.name;
            itemTypeText.text = "Consumable";
            itemDescriptionText.text = item.description;
            auxiliaryText.text = $"[Right Click] to use";

            containerObject.SetActive(false);
        }

        if (showPrice)
        {
            // Set to show price
            auxiliaryText.fontSize = 48f;
            auxiliaryText.text = $"Price: {item.GetValue()}<sprite name=\"Gold\">";
        }

        // Update any layouts
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        // Relocate
        transform.position = position;

        // Delay pivoting
        StartCoroutine(DelayedReveal());

        state = VisibilityState.Transitioning;
    }

    private IEnumerator DelayedReveal()
    {
        // Wait until end of frame to make sure content fitter has updated
        yield return new WaitForSeconds(delay);

        // Update pivot based on final size of window
        UpdatePivot();

        // Display window
        canvasGroup.alpha = 1f;

        state = VisibilityState.Visible;
    }

    private void UpdatePivot()
    {
        var width = rectTransform.sizeDelta.x;
        var height = rectTransform.sizeDelta.y;

        float pivotX = 1;
        float pivotY = 1;

        var screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        // Check if window goes off-screen on x-axis
        // If so, 
        if (screenPosition.x - width < 0) // if (anchorPosition.x + width > Screen.width)
        {
            // Change pivot to right of window
            pivotX = 0f;
        }

        // Check if window goes off-screen on y-axis
        // If so, flip vertically
        if (screenPosition.y - height < 0) // if (anchorPosition.y + height > Screen.height)
        {
            // Change pivot to center
            pivotY = 0.5f;
        }

        // Set updated pivot
        rectTransform.pivot = new Vector2(pivotX, pivotY);
    }

    public void Hide()
    {
        if (state == VisibilityState.Invisible) return;

        if (state == VisibilityState.Transitioning)
        {
            StopAllCoroutines();
        }

        actionsTabUI.Uninitialize();
        enchantmentsTabUI.Uninitialize();

        // Then disable window
        canvasGroup.alpha = 0f;

        state = VisibilityState.Invisible;
    }

    private void OnDrawGizmosSelected()
    {
        var rectTransform = GetComponent<RectTransform>();
        Gizmos.DrawWireSphere(rectTransform.pivot, 0.25f);
    }
}
