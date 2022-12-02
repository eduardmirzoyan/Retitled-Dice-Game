using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Die : ScriptableObject
{
    [Header("Data")]
    public int maxValue = 6;
    public int value = 1;
    public bool isExhausted = false;

    [Header("Debugging")]
    public bool neverExhaust = false;
    public bool lockValue = false;

    public void Exhaust() {
        if (neverExhaust) return;
        
        isExhausted = true;

        // Trigger event
        GameEvents.instance.TriggerOnDieExhaust(this);
    }

    public void Replenish() {
        isExhausted = false;
    }
    
    public void Roll() {
        if (lockValue) return;

        // Generate a random value
        value = Random.Range(1, maxValue + 1);
    }

    public Die Copy() {
        var copy = Instantiate(this);

        // Do more here if needed?
        copy.value = 1;
        copy.isExhausted = false;

        return copy;
    }
}
