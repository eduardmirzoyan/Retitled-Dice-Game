using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLoseUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Data")]
    [SerializeField] private float delayDuration = 1f;

    public static GameLoseUI instance;
    private void Awake()
    {
        // Singleton logic
        if (GameLoseUI.instance != null)
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
        GameEvents.instance.onGameLose += Show;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onGameLose -= Show;
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

        // Update score
        scoreText.text = "Score: " + DataManager.instance.GetPlayer().gold;
    }

    public void Restart()
    {
        // Create new character
        DataManager.instance.CreateNewPlayer();

        // Load new world
        GameManager.instance.TravelToNextFloor();
    }
}
