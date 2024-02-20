using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltipUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI auxiliaryText;

    [Header("Layouts")]
    [SerializeField] private LayoutGroup actionsLayoutGroup;
    [SerializeField] private LayoutGroup enchantmentsLayoutGroup;

    [Header("Seperators")]
    [SerializeField] private GameObject actionSeperator;
    [SerializeField] private GameObject enchantmentSeperator;
    [SerializeField] private GameObject priceSeperator;

    [Header("Prefabs")]
    [SerializeField] private GameObject actionTooltipPrefab;
    [SerializeField] private GameObject weaponEnchantmentTooltipPrefab;

    private List<WeaponActionUI> actionTooltips;
    private List<WeaponEnchantmentTooltipUI> enchantmentTooltips;

    private bool updatePivot;

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

        updatePivot = false;

        canvasGroup = GetComponentInChildren<CanvasGroup>();
        actionTooltips = new List<WeaponActionUI>();
        enchantmentTooltips = new List<WeaponEnchantmentTooltipUI>();
    }

    private void LateUpdate()
    {
        if (updatePivot)
        {
            UpdatePivot();
        }
    }

    private void UpdatePivot()
    {
        var width = rectTransform.sizeDelta.x;
        var height = rectTransform.sizeDelta.y;

        float pivotX = 1;
        float pivotY = 1;

        var screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        // Debug.Log($"Y Pos: {screenPosition.y}, Height: {height}, Screen: {Screen.height}");
        // Debug.Log($"Tras: {rectTransform}");
        // Debug.Log($"Pos: {rectTransform.position}");
        // Debug.Log($"Rect: {rectTransform.rect}");

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

        updatePivot = false;
    }

    public void Show(Item item, Vector3 position, bool showPrice = false)
    {
        // Display window
        canvasGroup.alpha = 1f;

        // Update information
        itemNameText.text = item.name;

        if (item is Weapon)
        {
            itemTypeText.text = "Weapon";

            var weapon = item as Weapon;
            itemDescriptionText.text = $"Base Damage: <color=yellow>{weapon.baseDamage}</color>";
            auxiliaryText.text = "";
        }
        else if (item is Consumable)
        {
            itemTypeText.text = "Consumable";
            itemDescriptionText.text = item.description;
            auxiliaryText.text = $"<color=yellow>[Right Click]</color> to Use";
        }

        if (showPrice)
        {
            // Set to show price
            auxiliaryText.fontSize = 48f;
            auxiliaryText.text = $"Price: {item.GetValue()}<sprite name=\"Gold\">";
        }

        // If the item is an equipment item
        if (item is Weapon)
        {
            // Cast
            var weapon = item as Weapon;

            // Display all its actions
            foreach (var action in weapon.actions)
            {
                // Spawn visuals of actions
                var weaponAction = Instantiate(actionTooltipPrefab, actionsLayoutGroup.transform).GetComponent<WeaponActionUI>();
                // Initialize as display
                weaponAction.Initialize(action);

                // Save
                actionTooltips.Add(weaponAction);
            }
            actionsLayoutGroup.gameObject.SetActive(weapon.actions.Count > 0);
            actionSeperator.SetActive(weapon.actions.Count > 0);

            // Display all its actions
            foreach (var enchantment in weapon.enchantments)
            {
                // Spawn visuals of actions
                var enchantmentTooltip = Instantiate(weaponEnchantmentTooltipPrefab, enchantmentsLayoutGroup.transform).GetComponent<WeaponEnchantmentTooltipUI>();
                enchantmentTooltip.Initialize(enchantment);

                // Save
                enchantmentTooltips.Add(enchantmentTooltip);
            }
            enchantmentsLayoutGroup.gameObject.SetActive(weapon.enchantments.Count > 0);
            enchantmentSeperator.SetActive(weapon.enchantments.Count > 0);
        }
        else if (item is Consumable)
        {
            actionsLayoutGroup.gameObject.SetActive(false);
            actionSeperator.SetActive(false);
            enchantmentsLayoutGroup.gameObject.SetActive(false);
            enchantmentSeperator.SetActive(false);
        }

        // Canvas.ForceUpdateCanvases();

        // Update Canvas
        LayoutRebuilder.ForceRebuildLayoutImmediate(actionsLayoutGroup.GetComponent<RectTransform>());

        // Update Canvas
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        // Relocate
        transform.position = position;
        // UpdatePivot();

        updatePivot = true;
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

        // Destroy all the ui
        foreach (var tooltip in enchantmentTooltips)
        {
            // Destroy
            Destroy(tooltip.gameObject);
        }
        enchantmentTooltips.Clear();

        // Then disable window
        canvasGroup.alpha = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        var rectTransform = GetComponent<RectTransform>();
        Gizmos.DrawWireSphere(rectTransform.pivot, 0.25f);
    }
}
