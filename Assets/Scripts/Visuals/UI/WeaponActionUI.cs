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
    [SerializeField] private TextMeshProUGUI dieMaxLabel;
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private TextMeshProUGUI actionDescriptionText;

    public void Initialize(Action action)
    {
        // Update Visuals
        actionIcon.sprite = action.icon;
        actionBackground.color = action.color;

        // Set basic info
        actionNameText.text = action.GetDynamicName();
        actionDescriptionText.text = action.GetInactiveDescription();

        // Set die label and color
        dieMaxLabel.text = $"{action.die.minValue}-{action.die.maxValue}";
        dieMaxLabel.color = action.color;
    }
}
