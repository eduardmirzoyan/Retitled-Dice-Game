using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameoverUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Data")]
    [SerializeField] private float delayDuration = 1f;
    private Entity entity;

    public static GameoverUI instance;
    private void Awake()
    {
        // Singleton logic
        if (GameoverUI.instance != null)
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
        GameEvents.instance.onEntityDespawn += ShowMenu;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEntityDespawn -= ShowMenu;
    }

    private void ShowMenu(Entity entity)
    {
        // If player was killed, show menu
        if (entity is Player)
        {
            this.entity = entity;
            StartCoroutine(DelayedDisplay(delayDuration));
        }
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
        scoreText.text = "Score: " + entity.gold;
    }

    public void Restart()
    {
        // Create new character
        DataManager.instance.CreateNewPlayer();

        // Load new world
        GameManager.instance.TravelToNextFloor();
    }
}
