using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ActionUpgradeUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private TextMeshProUGUI actionRangeText;
    [SerializeField] private Button upgradeActionButton;
    [SerializeField] private TextMeshProUGUI upgradeCostText;

    [Header("Dynamic Data")]
    [SerializeField] private Action action;
    [SerializeField] private Entity entity;

    [Header("Settings")]
    [SerializeField] private Color unafforableColor;

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
            bool affordable = entity.gold >= action.UpgradeCost();
            upgradeActionButton.interactable = affordable;

            // Update label
            actionNameText.text = action.GetDynamicName();
            actionRangeText.text = $"Range: <color=yellow>{action.die.minValue} - {action.die.maxValue}</color>";
            upgradeCostText.text = $"Cost: {action.UpgradeCost()}";

            string cost = affordable ? $"{action.UpgradeCost()}" : $"<color=#{unafforableColor.ToHexString()}>{action.UpgradeCost()}</color>";
            upgradeCostText.text = $"Cost: {cost}";
        }
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
