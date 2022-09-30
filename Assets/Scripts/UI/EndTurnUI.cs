using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EndTurnUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Start()
    {
        // Sub to events
        GameEvents.instance.onTurnStart += EnableButton;
        GameEvents.instance.onTurnEnd += DisableButton;
    }

    private void OnDestroy() {
        // Unsub to events
    }

    private void EnableButton(Entity entity)
    {
        if (entity is Player) {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        
    }

    private void DisableButton(Entity entity)
    {
        if (entity is Player)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // End current turn
        GameManager.instance.EndTurnNow();
    }
}
