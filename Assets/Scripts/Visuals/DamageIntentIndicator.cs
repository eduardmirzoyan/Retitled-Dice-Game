using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIntentIndicator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshPro damageLabel;

    [Header("Debug")]
    [SerializeField] private int damage;

    public void Initialize(int damage, Color color)
    {
        damageLabel.color = color;
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
}
