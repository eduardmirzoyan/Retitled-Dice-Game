using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        gameObject.name = $"{action.name} Action UI";

        // Sub to events
        GameEvents.instance.onToggleAllowAction += ToggleInteraction;
        GameEvents.instance.onActionSelect += SetSelected;
        GameEvents.instance.onDieRoll += UpdateLock;
        GameEvents.instance.onDieLock += UpdateLock;
    }

    public void Uninitialize()
    {
        diceUI.Uninitialize();

        // Unsub from events
        GameEvents.instance.onToggleAllowAction -= ToggleInteraction;
        GameEvents.instance.onActionSelect -= SetSelected;
        GameEvents.instance.onDieRoll -= UpdateLock;
        GameEvents.instance.onDieLock -= UpdateLock;
    }

    private void ToggleInteraction(bool state)
    {
        if (state)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            diceUI.Pulse();
        }
        else
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            diceUI.Idle();
        }
    }


    private void SetSelected(Entity entity, Action action)
    {
        if (entity is Player)
        {
            // Make visible
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            // If un-selected action
            if (action == null)
            {
                diceUI.Pulse();

                // Play Sound
                AudioManager.instance.PlaySFX("die_drop");
            }
            // If this action was selected
            else if (this.action == action)
            {
                diceUI.Highlight();

                // Play Sound
                AudioManager.instance.PlaySFX("die_pick");
            }
            else
            {
                diceUI.Idle();
            }
        }
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

        // Display tooltip from bottom-right corner
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



