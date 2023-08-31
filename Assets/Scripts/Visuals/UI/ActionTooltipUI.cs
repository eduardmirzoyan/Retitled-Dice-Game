using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionTooltipUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private Image actionIcon;
    [SerializeField] private Image actionBackground;
    [SerializeField] private TextMeshProUGUI dieMaxLabel;
    [SerializeField] private CanvasGroup descriptionCanvasGroup;
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private TextMeshProUGUI actionDescriptionText;

    public void Initialize(Action action)
    {
        // Update Visuals
        actionIcon.sprite = action.icon;
        actionBackground.sprite = action.background;

        // Set basic info
        actionNameText.text = action.name;
        actionDescriptionText.text = action.fullDescription;

        // Show description
        descriptionCanvasGroup.alpha = 1f;

        // Set die label and color
        dieMaxLabel.text = "MAX\n" + action.die.maxValue;
        dieMaxLabel.color = action.color;
    }
}
