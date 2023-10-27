using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionUpgradeUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private TextMeshProUGUI actionRangeText;
    [SerializeField] private Button upgradeMinButton;
    [SerializeField] private Button upgradeMaxButton;
    [SerializeField] private TextMeshProUGUI upgradeCostText;

    [Header("Dynamic Data")]
    [SerializeField] private Action action;
    [SerializeField] private Entity entity;

    public void Initialize(Action action, Entity entity)
    {
        this.action = action;
        this.entity = entity;

        // Set basic info
        actionNameText.text = action.GetDynamicName();
        actionRangeText.text = $"Range: {action.die.minValue} - {action.die.maxValue}";

        UpdatePrice(entity, 0);

        GameEvents.instance.onEntityGoldChange += UpdatePrice;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onEntityGoldChange -= UpdatePrice;
    }

    private void UpdatePrice(Entity entity, int change)
    {
        if (this.entity == entity)
        {
            // Check if buttons can be interacted with
            upgradeMaxButton.interactable = entity.gold >= action.UpgradeCost();
            upgradeMinButton.interactable = entity.gold >= action.UpgradeCost() && action.die.maxValue > action.die.minValue;

            // Update label
            actionNameText.text = action.GetDynamicName();
            actionRangeText.text = $"Range: {action.die.minValue} - {action.die.maxValue}";
            string goldIcon = "<sprite name=\"Gold\">";
            upgradeCostText.text = $"Cost: {action.UpgradeCost()}{goldIcon}";
        }
    }

    public void IncreaseMinValue()
    {
        int cost = -action.UpgradeCost();

        // Apply upgrade
        action.UpgradeMinValue();

        // Spend money
        entity.AddGold(cost);
    }

    public void IncreaseMaxValue()
    {
        int cost = -action.UpgradeCost();

        // Apply upgrade
        action.UpgradeMaxValue();

        // Spend money
        entity.AddGold(cost);

    }
}
