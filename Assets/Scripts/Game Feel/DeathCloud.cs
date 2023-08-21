using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCloud : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.25f;
    private void Awake() {
        // Start death timer
        Destroy(gameObject, animationDuration);
    }
}
