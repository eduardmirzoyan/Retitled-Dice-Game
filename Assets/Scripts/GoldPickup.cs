using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Data")]
    [SerializeField] private Vector3Int location;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(Vector3Int location) {
        this.location = location;

        // Sub to events
        GameEvents.instance.onPickup += Pickup;
    }

    private void OnDestroy() {
        // Unsub
        GameEvents.instance.onPickup -= Pickup;
    }

    private void Pickup(Entity entity, int index) {
        // If this coin was picked up destroy it
        if (this.location == entity.location && index == 2) {
            // Destroy coin
            Destroy(gameObject);
        }
        
    }
}
