using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private float testXVel = 0f;
    [SerializeField] private float testYVel = 0f;
    [SerializeField] private float testDuration = 1f;


    // Handles the need to display any form of text in the game.
    public static FloatingTextManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (FloatingTextManager.instance != null)
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
        GameEvents.instance.onEntityGainExperience += ShowExperienceNumber;
        GameEvents.instance.onEntityGainLevel += ShowLevelUp;
        GameEvents.instance.onEntityGainGold += ShowGoldNumber;
        GameEvents.instance.onPickup += ShowPickup;

    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEntityTakeDamage -= ShowDamageNumber;
        GameEvents.instance.onEntityGainExperience -= ShowExperienceNumber;
        GameEvents.instance.onEntityGainLevel -= ShowLevelUp;
        GameEvents.instance.onEntityGainGold -= ShowGoldNumber;
        GameEvents.instance.onPickup -= ShowPickup;
    }

    private void Update()
    {
        // Debugging
        if (Input.GetKeyDown(KeyCode.H))
        {
            var ft = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity).GetComponent<FloatingText>();
            ft.Initialize("Test", Color.cyan, testXVel, testYVel, testDuration);
        }
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
            SpawnFloatingText(entity, "+" + amount + " HP", Color.green, 0.5f, 2f, 1f);
        }
    }

    private void ShowExperienceNumber(Entity entity, int amount)
    {
        string message = "+" + amount + " XP";
        SpawnFloatingText(entity, message, Color.cyan, 0, 3f, 1f);
    }

    private void ShowLevelUp(Entity entity, int amount)
    {
        SpawnFloatingText(entity, "LEVEL UP!", Color.cyan, 0, 3f, 1f);
    }

    private void ShowPickup(Entity entity, int pickupIndex)
    {
        if (pickupIndex == 1) { // Key
            SpawnFloatingText(entity, "+Key", Color.yellow, 0, 3f, 1f);
        }
    }

    private void ShowGoldNumber(Entity entity, int amount) 
    {
        string message = "+" + amount + " G";
        SpawnFloatingText(entity, message, Color.yellow, 0, 3f, 1f);
    }

    private void SpawnFloatingText(Entity entity, string message, Color color, float xVel, float yVel, float duration)
    {
        var worldLocation = RoomUI.instance.GetLocationCenter(entity.location);
        var ft = Instantiate(floatingTextPrefab, worldLocation, Quaternion.identity).GetComponent<FloatingText>();
        ft.Initialize(message, color, xVel, yVel, duration);
    }

}
