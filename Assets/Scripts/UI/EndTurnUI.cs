using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EndTurnUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button button;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Start()
    {
        // Sub to events
        GameEvents.instance.onTurnStart += EnableButton;
        GameEvents.instance.onActionPerformStart += DisableButton;
        GameEvents.instance.onActionPerformEnd += EnableButton;
        GameEvents.instance.onTurnEnd += DisableButton;
    }

    private void OnDestroy()
    {
        // Unsub to events
        GameEvents.instance.onTurnStart -= EnableButton;
        GameEvents.instance.onActionPerformStart -= DisableButton;
        GameEvents.instance.onActionPerformEnd -= EnableButton;
        GameEvents.instance.onTurnEnd -= DisableButton;
    }

    private void EnableButton(Entity entity)
    {
        if (entity is Player)
        {
            canvasGroup.alpha = 1f;
            button.interactable = true;
        }
    }

    private void EnableButton(Entity entity, Action action, Vector3Int vector3Int, Dungeon dungeon) {
        EnableButton(entity);
    }

    private void DisableButton(Entity entity, Action action, Vector3Int vector3Int, Dungeon dungeon)
    {
        DisableButton(entity);
    }

    private void DisableButton(Entity entity)
    {
        if (entity is Player)
        {
            canvasGroup.alpha = 0.6f;
            button.interactable = false;
        }
    }
}
