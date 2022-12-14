using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public enum ActionMode { Interact, Display, Inspect }

public class ActionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    [SerializeField] private Image actionIcon;
    [SerializeField] private Image actionBackground;
    [SerializeField] private DieUI diceUI;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TooltipTriggerUI tooltipTriggerUI;
    [SerializeField] private CanvasGroup descriptionCanvasGroup;
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private TextMeshProUGUI actionDescriptionText;

    [Header("Data")]
    [SerializeField] private Action action;
    [SerializeField] private ActionMode actionMode;
    [SerializeField] private Entity entity;
    

    // 3 Modes, interactable, display, inspect

    public void Initialize(Action action, ActionMode actionMode, Entity entity = null)
    {
        this.action = action;
        this.actionMode = actionMode;
        this.entity = entity;

        // Update Visuals
        actionIcon.sprite = action.icon;
        actionBackground.sprite = action.background;

        // Initalize tooltip
        tooltipTriggerUI.SetTooltip(action.name, action.description);

        actionNameText.text = action.name;
        actionDescriptionText.text = action.description;

        switch (actionMode)
        {
            case ActionMode.Interact:
                // Initialize die
                diceUI.Initialize(action, true, false);
                // Hide description
                descriptionCanvasGroup.alpha = 0f;
                break;
            case ActionMode.Display:
                // Initialize die
                diceUI.Initialize(action, false, true);
                // Show description
                descriptionCanvasGroup.alpha = 1f;
                break;
            case ActionMode.Inspect:
                // Initialize die
                diceUI.Initialize(action, false, false);
                // Hide description
                descriptionCanvasGroup.alpha = 0f;
                break;
        }

        // Sub to events
        GameEvents.instance.onTurnStart += AllowInteraction;
        GameEvents.instance.onActionPerformStart += PreventInteraction;
        GameEvents.instance.onActionPerformEnd += AllowInteraction;
        GameEvents.instance.onTurnEnd += PreventInteraction;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // If the action is in inspect mode
        if (actionMode == ActionMode.Inspect && entity != null)
        {
            // Show preview
            GameEvents.instance.TriggerOnInspectAction(entity, action, entity.room);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // If the action is in inspect mode
        if (actionMode == ActionMode.Inspect && entity != null)
        {
            // Hide preview
            GameEvents.instance.TriggerOnInspectAction(entity, null, entity.room);
        }
    }

    public void Uninitialize()
    {
        // uninitialize die as well
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
        // Prevent touching of dice
        if (entity is Player)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void PreventInteraction(Entity entity, Action action, Vector3Int location, Room room)
    {
        // Prevent the touching of dice
        if (entity is Player)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
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

}
