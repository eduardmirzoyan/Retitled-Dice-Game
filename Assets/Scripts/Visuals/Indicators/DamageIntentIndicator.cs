using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIntentIndicator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshPro damageLabel;

    [Header("Settings")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private float defaultFontSize;
    [SerializeField] private float highlightedFontSize;

    [Header("Debug")]
    [SerializeField] private int damage;

    public void Initialize(int damage)
    {
        damageLabel.color = defaultColor;
        damageLabel.fontSize = defaultFontSize;
        SetValue(damage);
    }

    public void SetValue(int damage)
    {
        this.damage = damage;

        damageLabel.text = $"{damage}";
    }

    public int GetValue()
    {
        return damage;
    }

    public void SetHighlightState(bool state)
    {
        damageLabel.color = state ? highlightedColor : defaultColor;
        damageLabel.fontSize = state ? highlightedFontSize : defaultFontSize;
    }
}
