using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        actionBackground.sprite = action.background;

        // Initalize tooltip
        tooltipTriggerUI.SetTooltip(action.name, action.briefDescription);

        // Initialize die
        diceUI.Initialize(action, shortcut);

        // Update name
        gameObject.name = action.name + " Action UI";

        // Sub to events
        GameEvents.instance.onTurnStart += AllowInteraction;
        GameEvents.instance.onActionSelect += ToggleInteraction;
        GameEvents.instance.onActionPerformStart += PreventInteraction;
        GameEvents.instance.onActionPerformEnd += AllowInteraction;
        GameEvents.instance.onTurnEnd += PreventInteraction;

        // Start prevented
        canvasGroup.alpha = 0.6f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        diceUI.Idle();
    }

    public void Uninitialize()
    {
        diceUI.Uninitialize();

        // Unsub from events
        GameEvents.instance.onTurnStart -= AllowInteraction;
        GameEvents.instance.onActionSelect -= ToggleInteraction;
        GameEvents.instance.onActionPerformStart -= PreventInteraction;
        GameEvents.instance.onActionPerformEnd -= AllowInteraction;
        GameEvents.instance.onTurnEnd -= PreventInteraction;
    }

    private void AllowInteraction(Entity entity)
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

    private void AllowInteraction(Entity entity, Action action, Vector3Int location, Room room)
    {
        AllowInteraction(entity);
    }

    private void PreventInteraction(Entity entity)
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

    private void PreventInteraction(Entity entity, Action action, Vector3Int location, Room room)
    {
        PreventInteraction(entity);
    }

    private void ToggleInteraction(Entity entity, Action action)
    {
        if (entity is Player)
        {
            // If un-selected action
            if (action == null)
            {
                AllowInteraction(entity);
            }
            // If this action was selected
            else if (this.action == action)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                diceUI.Highlight();
            }
            else
            {
                PreventInteraction(entity);
            }
        }

    }

}



