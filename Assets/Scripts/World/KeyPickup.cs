using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Vector3Int location;
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private Vector3 exitLocation;

    [Header("Settings")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float travelSpeed;

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
            // Play sound
            AudioManager.instance.PlaySFX("gold");

            // Spawn effect
            // Instantiate(pickupEffect, transform.position, Quaternion.identity);

            // Destroy key
            Destroy(gameObject);

            // Vector3 exitPositon = RoomManager.instance.GetExitPosition();
            // Vector3 startLocation = transform.position;

            // Fly to target
            // StartCoroutine(FlyTowardTarget(startLocation, exitPositon, travelSpeed));
        }
    }

    private IEnumerator FlyTowardTarget(Vector3 start, Vector3 end, float duration)
    {
        // Fly toward target while spinning
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            transform.Rotate(Vector3.forward * rotationSpeed);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Destroy coin
        Destroy(gameObject);
    }
}
