using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image actionIcon;
    [SerializeField] private Image actionBackground;
    [SerializeField] private DieUI diceUI;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Data")]
    [SerializeField] private Action action;

    public void Initialize(Action action)
    {
        this.action = action;

        // Update Visuals
        actionIcon.sprite = action.icon;
        actionBackground.sprite = action.background;

        // Initialize die
        diceUI.Initialize(action.die, action);

        // Sub to events
        GameEvents.instance.onTurnStart += AllowInteraction;
        GameEvents.instance.onActionPerformStart += PreventInteraction;
        GameEvents.instance.onActionPerformEnd += AllowInteraction;
        GameEvents.instance.onTurnEnd += PreventInteraction;
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

    private void AllowInteraction(Entity entity, Action action, Vector3Int location, Dungeon dungeon)
    {
        // Prevent touching of dice
        if (entity is Player)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void PreventInteraction(Entity entity, Action action, Vector3Int location, Dungeon dungeon)
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
