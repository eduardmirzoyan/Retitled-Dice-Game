using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExitUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer iconRenderer;

    [Header("Data")]
    [SerializeField] private RoomExit roomExit;
    [SerializeField] private List<Sprite> exitIcons;

    public void Initialize(RoomExit roomExit)
    {
        this.roomExit = roomExit;

        // If door is already unlocked
        if (!roomExit.IsLocked())
        {
            // Play animation
            animator.Play("Unlock");
        }

        // Set sprite based on what the exit is to
        switch (roomExit.destinationIndex)
        {
            case 1:
                iconRenderer.sprite = exitIcons[0];
                break;
            case -1:
                iconRenderer.sprite = exitIcons[1];
                break;
        }

        // Sub to events
        GameEvents.instance.onUseKey += CheckUnlock;
    }

    private void OnDestroy()
    {
        // Unsub to events
        GameEvents.instance.onUseKey -= CheckUnlock;
    }

    public void CheckUnlock(int value)
    {
        if (!roomExit.IsLocked())
        {
            // Unlock door
            animator.Play("Unlock");
        }
    }
}
