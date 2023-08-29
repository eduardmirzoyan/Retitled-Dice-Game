using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameWinUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Data")]
    [SerializeField] private float delayDuration = 1f;

    public static GameWinUI instance;
    private void Awake()
    {
        // Singleton logic
        if (GameWinUI.instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Start()
    {
        // Sub to events
        GameEvents.instance.onGameWin += Show;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onGameWin -= Show;
    }

    private void Show()
    {
        StartCoroutine(DelayedDisplay(delayDuration));
    }

    private IEnumerator DelayedDisplay(float duration)
    {
        // Wait
        yield return new WaitForSeconds(duration);

        // Show screen
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Continue()
    {
        // Load new world
        GameManager.instance.TravelToNextFloor();
    }

    public void Restart()
    {
        // Create new character
        DataManager.instance.CreateNewPlayer();

        // Load new world
        GameManager.instance.TravelToNextFloor();
    }
}
