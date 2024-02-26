using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("Bonuses")]
    public int bonusMinRoll;
    public int bonusMaxRoll;
    public bool isLocked;

    [Header("Debug")]
    public bool neverExhaust = false;
    public bool alwaysLock = false;

    public int TrueMax
    {
        get
        {
            return maxValue + bonusMaxRoll;
        }
    }

    public int TrueMin
    {
        get
        {
            return minValue + bonusMinRoll;
        }
    }

    public void Initialize(Action action)
    {
        this.action = action;

        // Start at min value and exhausted
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
        if (alwaysLock) return;

        if (isLocked)
        {
            isLocked = false;
        }
        else
        {
            // Generate a random value
            value = Random.Range(TrueMin, TrueMax + 1);
        }

        // Trigger events
        GameEvents.instance.TriggerOnDieRoll(this);
    }

    public void Bump(int amount)
    {
        if (alwaysLock) return;

        if (isLocked)
        {
            isLocked = false;
            return;
        }
        else
        {
            value = Mathf.Min(value + amount, TrueMax);
        }

        GameEvents.instance.TriggerOnDieBump(this);
    }

    public void Lock()
    {
        isLocked = true;

        GameEvents.instance.TriggerOnDieLock(this);
    }

    public Die Copy()
    {
        var copy = Instantiate(this);

        return copy;
    }
}
