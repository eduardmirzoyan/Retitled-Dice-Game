using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
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
        GameEvents.instance.onPickup += Pickup;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onPickup -= Pickup;
    }

    private void Pickup(Entity entity, int index)
    {
        // If this key was picked up destroy it
        if (this.location == entity.location && index == 1)
        {
            // Spawn effect
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

            // Destroy coin
            Destroy(gameObject);
        }
    }
}
