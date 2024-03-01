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
        GameEvents.instance.onActionPerformEnd += Highlight;
        GameEvents.instance.onToggleAllowAction += ToggleButton;
    }

    private void OnDestroy()
    {
        // Unsub to events
        GameEvents.instance.onActionPerformEnd -= Highlight;
        GameEvents.instance.onToggleAllowAction -= ToggleButton;
    }

    private void Update()
    {
        if (button.interactable && Input.GetKeyDown(endTurnButton))
        {
            // Invoke the button click
            button.onClick.Invoke();
        }
    }

    private void ToggleButton(bool state)
    {
        canvasGroup.alpha = state ? 1f : 0.6f;
        button.interactable = state;

        if (!state)
            outline.enabled = false;
    }

    private void Highlight(Entity entity, Action action, Vector3Int vector3Int, Room room)
    {
        // Check if any possible moves are left, if not, then highlight
        if (entity is Player)
            outline.enabled = entity.AllActions().All(action => action.die.isExhausted);
    }
}
