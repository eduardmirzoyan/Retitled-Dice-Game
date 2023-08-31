using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Vector3Int location;
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private Vector3 exitLocation;

    public void Initialize(Vector3Int location)
    {
        this.location = location;

        // Sub to events
        GameEvents.instance.onPickupDespawn += Pickup;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onPickupDespawn -= Pickup;
    }

    private void Pickup(Vector3Int location)
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

    private IEnumerator FlyTowardExit(Vector3 start, Vector3 end, float duration)
    {
        // TODO
        yield return null;

        // Destroy coin
        Destroy(gameObject);
    }
}
