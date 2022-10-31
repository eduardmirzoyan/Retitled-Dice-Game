using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Die : ScriptableObject
{
    public int maxValue = 6;
    public int value = 1;
    public bool isExhausted = false;

    public bool hasCheats = false;

    public void Exhaust() {
        if (hasCheats) return;
        
        isExhausted = true;
    }

    public void Replenish() {
        isExhausted = false;
    }
    
    public void Roll() {
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
