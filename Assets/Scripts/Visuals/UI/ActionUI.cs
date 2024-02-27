using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Static Data")]
    [SerializeField] private Image actionIcon;
    [SerializeField] private Image actionBackground;
    [SerializeField] private DieUI diceUI;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Outline actionOutline;
    [SerializeField] private Image lockIcon;

    [Header("Dynamic Data")]
    [SerializeField] private Action action;
    [SerializeField] private KeyCode hotkey;

    public void Initialize(Action action, KeyCode hotkey = KeyCode.None)
    {
        this.action = action;
        this.hotkey = hotkey;

        // Update Visuals
        actionIcon.sprite = action.icon;
        actionBackground.color = action.color;

        // Initialize die
        diceUI.Initialize(action, hotkey);
        UpdateLock(action.die);

        // Show
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (action.die.isExhausted)
            diceUI.Idle();
        else
            diceUI.Pulse();

        // Update name
        gameObject.name = action.name + " Action UI";

        // Sub to events
        GameEvents.instance.onTurnStart += SetInteractable;
        GameEvents.instance.onActionPerformEnd += SetInteractable;
        GameEvents.instance.onActionSelect += SetSelected;
        GameEvents.instance.onActionPerformStart += SetUninteractable;
        GameEvents.instance.onTurnEnd += SetUninteractable;

        GameEvents.instance.onDieRoll += UpdateLock;
        GameEvents.instance.onDieLock += UpdateLock;
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

        GameEvents.instance.onDieRoll -= UpdateLock;
        GameEvents.instance.onDieLock -= UpdateLock;
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

                // Play Sound
                AudioManager.instance.PlaySFX("die_drop");
                return;
            }

            // If this action was selected
            if (this.action == action)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                diceUI.Highlight();

                // Play Sound
                AudioManager.instance.PlaySFX("die_pick");
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

    private void UpdateLock(Die die)
    {
        if (this.action.die == die)
        {
            lockIcon.enabled = die.isLocked;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        actionOutline.enabled = true;
        var corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        ActionTooltipUI.instance.Show(action, hotkey.ToString().Replace("Alpha", ""), corners[3]);

        // Play sfx
        AudioManager.instance.PlayFiller();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        actionOutline.enabled = false;
        ActionTooltipUI.instance.Hide();
    }
}



