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

    public void Initialize(Action action)
    {
        // Update Visuals
        actionIcon.sprite = action.icon;
        actionBackground.sprite = action.background;

        // Initalize tooltip
        tooltipTriggerUI.SetTooltip(action.name, action.briefDescription);

        // Initialize die
        diceUI.Initialize(action, true, false);

        // Sub to events
        GameEvents.instance.onTurnStart += AllowInteraction;
        GameEvents.instance.onActionPerformStart += PreventInteraction;
        GameEvents.instance.onActionPerformEnd += AllowInteraction;
        GameEvents.instance.onTurnEnd += PreventInteraction;
    }

    public void Uninitialize()
    {
        diceUI.Uninitialize();

        // Unsub from events
        GameEvents.instance.onTurnStart -= AllowInteraction;
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
        }
    }

    private void PreventInteraction(Entity entity, Action action, Vector3Int location, Room room)
    {
        PreventInteraction(entity);
    }

}



