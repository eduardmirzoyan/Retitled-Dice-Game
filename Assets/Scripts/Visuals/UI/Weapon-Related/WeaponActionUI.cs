using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponActionUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private Image actionIcon;
    [SerializeField] private Image actionBackground;
    [SerializeField] private TextMeshProUGUI actionNameLabel;
    [SerializeField] private TextMeshProUGUI actionTypeLabel;
    [SerializeField] private TextMeshProUGUI actionDescriptionLabel;
    [SerializeField] private TextMeshProUGUI dieMaxLabel;

    public void Initialize(Action action)
    {
        // Update Visuals
        actionIcon.sprite = action.icon;
        actionBackground.color = action.color;

        // Set basic info
        actionNameLabel.text = action.GetDynamicName();
        actionTypeLabel.text = $"{action.actionType} Action";
        actionTypeLabel.color = action.color;
        actionDescriptionLabel.text = action.GetInactiveDescription();

        // Set die label and color
        dieMaxLabel.text = $"{action.die.minValue}-{action.die.maxValue}";
        dieMaxLabel.color = action.color;
    }
}
