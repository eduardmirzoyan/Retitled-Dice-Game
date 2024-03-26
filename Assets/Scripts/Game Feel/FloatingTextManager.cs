using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    [SerializeField] private Canvas playerScreen;

    [SerializeField] private GameObject floatingUIPrefab;
    [SerializeField] private GameObject floatingTextPrefab;

    // Handles the need to display any form of text in the game.
    public static FloatingTextManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        // Sub
        GameEvents.instance.onEntityTakeDamage += ShowDamageNumber;
        GameEvents.instance.onEntityGoldChange += ShowGoldNumber;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEntityTakeDamage -= ShowDamageNumber;
        GameEvents.instance.onEntityGoldChange -= ShowGoldNumber;
    }

    private void ShowDamageNumber(Entity entity, int amount)
    {
        if (amount > 0)
        {
            // Spawn as damage number
            SpawnFloatingText(entity, "" + amount, Color.red, 0.5f, 2f, 1f);
        }
        else
        {
            // Spawn as heal number
            SpawnFloatingText(entity, $"+{-amount} HP", Color.green, 0.5f, 2f, 1f);
        }
    }

    private void ShowGoldNumber(Entity entity, int amount)
    {
        string message = $"+{amount} G";
        SpawnFloatingText(entity, message, Color.yellow, 0, 3f, 1f);
    }

    private void SpawnFloatingText(Entity entity, string message, Color color, float xVel, float yVel, float duration)
    {
        var worldLocation = RoomManager.instance.GetLocationCenter(entity.location);
        var ft = Instantiate(floatingTextPrefab, worldLocation, Quaternion.identity).GetComponent<FloatingText>();
        ft.Initialize(message, color, xVel, yVel, duration);
    }

    public void SpawnFeedbackText(string message)
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        var fb = Instantiate(floatingUIPrefab, mousePosition, Quaternion.identity, playerScreen.transform).GetComponent<FeedbackTextUI>();
        fb.Initialize(message);
    }
}
