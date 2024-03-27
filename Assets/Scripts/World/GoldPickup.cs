using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Vector3Int location;
    [SerializeField] private GameObject pickupEffect;

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
            // Play sound
            AudioManager.instance.PlaySFX("gold");

            // Spawn effect
            // Instantiate(pickupEffect, transform.position, Quaternion.identity);

            // Destroy coin
            Destroy(gameObject);
        }
    }
}
