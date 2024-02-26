using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
        // Start disabled
        canvasGroup.alpha = 0.6f;
        button.interactable = false;

        // Sub to events
        GameEvents.instance.onTurnStart += EnableButton;
        GameEvents.instance.onActionPerformStart += DisableButton;
        GameEvents.instance.onActionPerformEnd += EnableButton;
        GameEvents.instance.onActionPerformEnd += Highlight;
        GameEvents.instance.onTurnEnd += DisableButton;
    }

    private void OnDestroy()
    {
        // Unsub to events
        GameEvents.instance.onTurnStart -= EnableButton;
        GameEvents.instance.onActionPerformStart -= DisableButton;
        GameEvents.instance.onActionPerformEnd -= EnableButton;
        GameEvents.instance.onActionPerformEnd -= Highlight;
        GameEvents.instance.onTurnEnd -= DisableButton;
    }

    private void Update()
    {
        if (button.interactable && Input.GetKeyDown(endTurnButton))
        {
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
        }
    }

    private void DisableButton(Entity entity)
    {
        if (entity is Player)
        {
            canvasGroup.alpha = 0.6f;
            button.interactable = false;

            // Disable any highlight
            outline.enabled = false;
        }
    }


    private void Highlight(Entity entity, Action action, Vector3Int vector3Int, Room room)
    {
        // Check if any possible moves are left, if not, then highlight
        if (entity is Player)
            outline.enabled = entity.AllActions().All(action => action.die.isExhausted);
    }


    private void EnableButton(Entity entity, Action action, Vector3Int vector3Int, Room room)
    {
        EnableButton(entity);
    }

    private void DisableButton(Entity entity, Action action, Vector3Int vector3Int, Room room)
    {
        DisableButton(entity);
    }


}
