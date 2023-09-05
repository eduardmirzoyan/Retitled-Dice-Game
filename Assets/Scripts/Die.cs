using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Die : ScriptableObject
{
    [Header("Dynamic Data")]
    public int maxValue = 6;
    public int minValue = 1;
    public int value = 1;
    public bool isExhausted = false;
    public Action action;

    [Header("Debugging")]
    public bool neverExhaust = false;
    public bool lockValue = false;

    public void Initialize(Action action)
    {
        this.action = action;

        // Start at min value and exhausted
        value = minValue;
        isExhausted = true;
    }

    public void Uninitialize()
    {
        this.action = null;
    }

    public void Exhaust()
    {
        if (neverExhaust) return;

        isExhausted = true;

        // Trigger event
        GameEvents.instance.TriggerOnDieExhaust(this);
    }

    public void Replenish()
    {
        isExhausted = false;

        // Trigger event
        GameEvents.instance.TriggerOnDieReplenish(this);
    }

    public void Roll()
    {
        if (lockValue) return;

        // Generate a random value
        value = Random.Range(minValue, maxValue + 1);

        // Trigger events
        GameEvents.instance.TriggerOnDieRoll(this);
    }

    public bool IsHighRoll()
    {
        return value == maxValue;
    }

    public bool IsLowRoll()
    {
        return value == minValue;
    }

    public Die Copy()
    {
        var copy = Instantiate(this);

        return copy;
    }
}
