using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUDUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image playerIcon;
    [SerializeField] private Transform heartContainer;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TextMeshProUGUI experienceText;

    [Header("Data")]
    [SerializeField] private Entity entity;
    [SerializeField] private GameObject heartPrefab;

    private List<GameObject> heartObjects;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        heartObjects = new List<GameObject>();
    }

    private void Start()
    {
        // Sub
        GameEvents.instance.onSpawnEntity += InitializePlayer;
        GameEvents.instance.onEntityTakeDamage += UpdateHealth;
        GameEvents.instance.onPickup += UpdateGold;
        GameEvents.instance.onEntityGainExperience += UpdateExperience;
        GameEvents.instance.onEntityGainLevel += UpdateLevel;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onSpawnEntity -= InitializePlayer;
        GameEvents.instance.onEntityTakeDamage -= UpdateHealth;
        GameEvents.instance.onPickup -= UpdateGold;
        GameEvents.instance.onEntityGainExperience -= UpdateExperience;
        GameEvents.instance.onEntityGainLevel -= UpdateLevel; 
    }

    public void InitializePlayer(Entity entity)
    {
        // If player is spawned
        if (entity is Player)
        {
            // Save
            this.entity = entity;

            // Update visuals
            UpdateIcon();
            UpdateHealth(entity, 0);
            UpdateGold(entity, 2);
            UpdateExperience(entity, 0);
            UpdateLevel(entity, 0);

            // Display
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void UpdateIcon()
    {
        playerIcon.sprite = entity.sprite;
    }

    public void UpdateHealth(Entity entity, int amount)
    {
        if (this.entity == entity)
        {
            // Destroy all hearts
            foreach (var ob in heartObjects)
            {
                Destroy(ob);
            }
            // Clear
            heartObjects.Clear();

            // Spawn new hearts
            for (int i = 0; i < entity.currentHealth; i++)
            {
                // Create heart
                heartObjects.Add(Instantiate(heartPrefab, heartContainer));
            }
        }

    }

    private void UpdateGold(Entity entity, int index)
    {
        // If gold was picked up
        if (this.entity == entity && index == 2)
        {
            goldText.text = "" + entity.gold;
        }
    }

    public void UpdateExperience(Entity entity, int amount)
    {
        if (this.entity == entity)
        {
            experienceSlider.value = entity.experience;
            experienceText.text  = entity.experience + " / 10 XP";
            
        }

    }

    public void UpdateLevel(Entity entity, int amount)
    {
        if (this.enabled == entity)
        {
            levelText.text = "Lv. " + entity.level;
        }

    }
}
