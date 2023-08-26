using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExitUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer iconRenderer;

    [Header("Data")]
    [SerializeField] private List<Sprite> exitIcons;

    private void Start()
    {
        GameEvents.instance.onLockExit += Lock;
        GameEvents.instance.onUnlockExit += Unlock;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onLockExit -= Lock;
        GameEvents.instance.onUnlockExit -= Unlock;
    }

    public void Lock()
    {
        // Remove icon while locked
        iconRenderer.sprite = null;

        // Unlock door
        animator.Play("Lock");
    }

    private void Unlock()
    {
        // Get next room index
        var nextRoomIndex = DataManager.instance.GetNextRoomIndex();

        // Set sprite based on what the exit is to
        switch (nextRoomIndex)
        {
            case 1:
                iconRenderer.sprite = exitIcons[0];
                break;
            case -1:
                iconRenderer.sprite = exitIcons[1];
                break;
        }

        // Unlock door
        animator.Play("Unlock");
    }
}
