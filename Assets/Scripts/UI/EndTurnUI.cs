using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EndTurnUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Outline outline;
    [SerializeField] private Button button;

    [Header("Components")]
    [SerializeField] private KeyCode endTurnButton = KeyCode.Space;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        outline = GetComponentInChildren<Outline>();
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

    private void Update() {
        if (button.interactable && Input.GetKeyDown(endTurnButton)) {
            // Invoke the button click
            button.onClick.Invoke();
        }
    }

    private void EnableButton(Entity entity)
    {
        if (entity is Player)
        {
            canvasGroup.alpha = 1f;
            button.interactable = true;

            // Check if any possible moves are left, if not, then highlight
            outline.enabled = entity.HasNoActionsLeft();
        }
    }

    private void EnableButton(Entity entity, Action action, Vector3Int vector3Int, Room dungeon) {
        EnableButton(entity);
    }

    private void DisableButton(Entity entity, Action action, Vector3Int vector3Int, Room dungeon)
    {
        DisableButton(entity);
    }

    private void DisableButton(Entity entity)
    {
        if (entity is Player)
        {
            canvasGroup.alpha = 0.6f;
            button.interactable = false;

            // Disable highlight
            outline.enabled = false;
        }
    }
}
