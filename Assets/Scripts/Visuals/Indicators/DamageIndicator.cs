using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIndicator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshPro damageLabel;

    public void Initialize(int damage, Color color)
    {
        damageLabel.color = color;
        damageLabel.text = $"{damage}";
    }

    public void Initialize(string text, Color color)
    {
        damageLabel.color = color;
        damageLabel.text = $"{text}";
    }
}
