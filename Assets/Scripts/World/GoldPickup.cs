using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Data")]
    [SerializeField] private Vector3Int location;
    [SerializeField] private GameObject pickupEffect;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(Vector3Int location)
    {
        this.location = location;

        // Sub to events
        GameEvents.instance.onPickupDespawn += Despawn;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onPickupDespawn -= Despawn;
    }

    private void Despawn(Vector3Int location)
    {
        // If this key was picked up destroy it
        if (this.location == location)
        {
            // Spawn effect
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

            // Destroy coin
            Destroy(gameObject);
        }
    }
}
