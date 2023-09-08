using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private Image actionIcon;
    [SerializeField] private Image actionBackground;
    [SerializeField] private DieUI diceUI;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TooltipTriggerUI tooltipTriggerUI;

    [Header("Dynamic Data")]
    [SerializeField] private Action action;

    public void Initialize(Action action, KeyCode shortcut = KeyCode.None)
    {
        this.action = action;

        // Update Visuals
        actionIcon.sprite = action.icon;
        actionBackground.color = action.color;

        // Initalize tooltip
        tooltipTriggerUI.SetTooltip(action.name + " [" + shortcut + "]", action.briefDescription);

        // Initialize die
        diceUI.Initialize(action, shortcut);

        // Update name
        gameObject.name = action.name + " Action UI";

        // Sub to events
        GameEvents.instance.onTurnStart += SetInteractable;
        GameEvents.instance.onActionPerformEnd += SetInteractable;

        GameEvents.instance.onActionSelect += SetSelected;

        GameEvents.instance.onActionPerformStart += SetUninteractable;
        GameEvents.instance.onTurnEnd += SetUninteractable;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (action.die.isExhausted)
            diceUI.Idle();
        else
            diceUI.Pulse();
    }

    public void Uninitialize()
    {
        diceUI.Uninitialize();

        // Unsub from events
        GameEvents.instance.onTurnStart -= SetInteractable;
        GameEvents.instance.onActionPerformEnd -= SetInteractable;

        GameEvents.instance.onActionSelect -= SetSelected;

        GameEvents.instance.onActionPerformStart -= SetUninteractable;
        GameEvents.instance.onTurnEnd -= SetUninteractable;
    }

    private void SetInteractable(Entity entity)
    {
        // Allow the touching of dice
        if (entity is Player)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            diceUI.Pulse();
        }
    }

    private void SetInteractable(Entity entity, Action action, Vector3Int location, Room room)
    {
        SetInteractable(entity);
    }

    private void SetSelected(Entity entity, Action action)
    {
        if (entity is Player)
        {
            // If un-selected action
            if (action == null)
            {
                SetInteractable(entity);
                return;
            }

            // If this action was selected
            if (this.action == action)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                diceUI.Highlight();
            }
            else
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                diceUI.Idle();
            }
        }

    }

    private void SetUninteractable(Entity entity)
    {
        // Prevent the touching of dice
        if (entity is Player)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            diceUI.Idle();
        }
    }

    private void SetUninteractable(Entity entity, Action action, Vector3Int location, Room room)
    {
        SetUninteractable(entity);
    }

}



