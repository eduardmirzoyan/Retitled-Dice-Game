using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltipUI : MonoBehaviour
{
    private enum TabMode { Actions, Enchantments }

    [Header("Static Data")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI auxiliaryText;
    [SerializeField] private TextMeshProUGUI instructionsText;

    [Header("Prefabs")]
    [SerializeField] private GameObject weaponActionPrefab;
    [SerializeField] private GameObject weaponEnchantmentPrefab;

    [Header("Tab Components")]
    [SerializeField] private GameObject containerObject;
    [SerializeField] private TabMode tabMode;
    [SerializeField] private GameObject actionsObject;
    [SerializeField] private GameObject enchantmentsObject;
    [SerializeField] private Image actionTabBackground;
    [SerializeField] private Image enchantmentTabBackground;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.LeftShift;
    [SerializeField] private Color activeTabColor;
    [SerializeField] private Color inactiveTabColor;

    [Header("Debug")]
    [SerializeField] private bool isVisible;

    private List<WeaponActionUI> actionTooltips;
    private List<WeaponEnchantmentTooltipUI> enchantmentTooltips;

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
        isVisible = false;

        canvasGroup = GetComponentInChildren<CanvasGroup>();
        actionTooltips = new List<WeaponActionUI>();
        enchantmentTooltips = new List<WeaponEnchantmentTooltipUI>();
    }

    private void Update()
    {
        if (isVisible)
        {
            if (Input.GetKey(toggleKey))
            {
                if (tabMode == TabMode.Actions)
                {
                    actionTabBackground.color = inactiveTabColor;
                    enchantmentTabBackground.color = activeTabColor;

                    actionsObject.SetActive(false);
                    enchantmentsObject.SetActive(true);

                    instructionsText.text = "Release [Shift] to switch tabs";

                    LayoutRebuilder.ForceRebuildLayoutImmediate(enchantmentsObject.GetComponent<RectTransform>());
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

                    tabMode = TabMode.Enchantments;
                }
            }
            else if (tabMode == TabMode.Enchantments)
            {
                actionTabBackground.color = activeTabColor;
                enchantmentTabBackground.color = inactiveTabColor;

                actionsObject.SetActive(true);
                enchantmentsObject.SetActive(false);

                instructionsText.text = "Hold [Shift] to switch tabs";

                LayoutRebuilder.ForceRebuildLayoutImmediate(actionsObject.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

                tabMode = TabMode.Actions;
            }
        }
    }

    public void Show(Item item, Vector3 position, bool showPrice = false)
    {
        if (isVisible) return;

        if (item is Weapon)
        {
            DisplayWeapon(item as Weapon);
        }
        else if (item is Consumable)
        {
            DisplayConsumable(item as Consumable);
        }

        if (showPrice)
        {
            // Set to show price
            auxiliaryText.fontSize = 48f;
            auxiliaryText.text = $"Price: {item.GetValue()}<sprite name=\"Gold\">";
        }

        // Update any layouts
        LayoutRebuilder.ForceRebuildLayoutImmediate(actionsObject.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(enchantmentsObject.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        // Relocate
        transform.position = position;

        // Delay pivoting
        StartCoroutine(DelayedReveal());

        isVisible = true;
    }

    private void DisplayWeapon(Weapon weapon)
    {
        itemNameText.text = weapon.name;
        itemTypeText.text = "Weapon";
        itemDescriptionText.text = $"Base Damage: <color=yellow>{weapon.baseDamage}</color>";
        auxiliaryText.text = "";

        containerObject.SetActive(true);

        // Display actions
        foreach (var action in weapon.actions)
        {
            // Spawn visuals of actions
            var weaponAction = Instantiate(weaponActionPrefab, actionsObject.transform).GetComponent<WeaponActionUI>();
            weaponAction.Initialize(action);

            // Save
            actionTooltips.Add(weaponAction);
        }

        // Display enchantments
        foreach (var enchantment in weapon.enchantments)
        {
            // Spawn visuals of actions
            var enchantmentTooltip = Instantiate(weaponEnchantmentPrefab, enchantmentsObject.transform).GetComponent<WeaponEnchantmentTooltipUI>();
            enchantmentTooltip.Initialize(enchantment);

            // Save
            enchantmentTooltips.Add(enchantmentTooltip);
        }
    }

    private void DisplayConsumable(Consumable consumable)
    {
        itemNameText.text = consumable.name;
        itemTypeText.text = "Consumable";
        itemDescriptionText.text = consumable.description;
        auxiliaryText.text = $"[Right Click] to use";

        containerObject.SetActive(false);
    }

    private IEnumerator DelayedReveal()
    {
        // Wait until end of frame to make sure content fitter has updated
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // Update pivot based on final size of window
        UpdatePivot();
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

        // Display window
        canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        if (!isVisible) return;

        // Destroy all the ui
        foreach (var tooltip in actionTooltips)
        {
            // Destroy
            Destroy(tooltip.gameObject);
        }
        actionTooltips.Clear();

        // Destroy all the ui
        foreach (var tooltip in enchantmentTooltips)
        {
            // Destroy
            Destroy(tooltip.gameObject);
        }
        enchantmentTooltips.Clear();

        // Then disable window
        canvasGroup.alpha = 0f;

        isVisible = false;
    }

    private void OnDrawGizmosSelected()
    {
        var rectTransform = GetComponent<RectTransform>();
        Gizmos.DrawWireSphere(rectTransform.pivot, 0.25f);
    }
}
