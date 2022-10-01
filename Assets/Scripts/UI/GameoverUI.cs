using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameoverUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Data")]
    [SerializeField] private float delayDuration = 1f;

    public static GameoverUI instance;
    private void Awake() {
        // Singleton logic
        if (GameoverUI.instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Start() {
        // Sub to events
        GameEvents.instance.onRemoveEntity += ShowMenu;
    }

    private void OnDestroy() {
        // Unsub
        GameEvents.instance.onRemoveEntity -= ShowMenu;
    }

    private void ShowMenu(Entity entity) {
        // If player was killed, show menu
        if (entity is Player) {
            StartCoroutine(DelayedDisplay(delayDuration));
        }
    }

    private IEnumerator DelayedDisplay(float duration) {
        // Wait
        yield return new WaitForSeconds(duration);

        // Show screen
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Restart() {
        // Create new character
        DataManager.instance.CreateNewPlayer();

        // Load new world
        GameManager.instance.TravelToNextFloor();
    }
}
