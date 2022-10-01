using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorExit : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Data")]
    [SerializeField] private Vector3Int location;
    [SerializeField] private bool isLocked;

    private void Awake() {
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(Vector3Int location) {
        this.location = location;
        isLocked = true;

        // Lock door
        animator.Play("Lock");

        // Sub to events
        GameEvents.instance.onUnlockExit += Unlock;
    }

    private void OnDestroy() {
        // Unsub to events
        GameEvents.instance.onUnlockExit -= Unlock;
    }

    public void Unlock(Vector3Int location) {
        if (this.location == location) {
            isLocked = false;

            // Unlock door
            animator.Play("Unlock");
        }
        
    }
}
