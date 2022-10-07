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
    [SerializeField] private TextMeshProUGUI floorNumberText;

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
        GameEvents.instance.onEnterFloor += UpdateFloor;
        GameEvents.instance.onEntityTakeDamage += UpdateHealth;
        GameEvents.instance.onEntityGainGold += UpdateGold;
        GameEvents.instance.onEntityGainExperience += UpdateExperience;
        GameEvents.instance.onEntityGainLevel += UpdateLevel;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onSpawnEntity -= InitializePlayer;
        GameEvents.instance.onEnterFloor -= UpdateFloor;
        GameEvents.instance.onEntityTakeDamage -= UpdateHealth;
        GameEvents.instance.onEntityGainGold -= UpdateGold;
        GameEvents.instance.onEntityGainExperience -= UpdateExperience;
        GameEvents.instance.onEntityGainLevel -= UpdateLevel;
    }

    private void InitializePlayer(Entity entity)
    {
        // If player is spawned
        if (entity is Player)
        {
            // Save
            this.entity = entity;

            // Update visuals
            UpdateIcon();
            UpdateHealth(entity, 0);
            UpdateGold(entity, 0);
            UpdateExperience(entity, 0);
            UpdateLevel(entity, 0);
            UpdateFloor(null);

            // Display
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void UpdateIcon()
    {
        playerIcon.sprite = entity.modelSprite;
    }

    private void UpdateHealth(Entity entity, int amount)
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

    private void UpdateGold(Entity entity, int amount)
    {
        // If non-zero gold was gained
        if (this.entity == entity)
        {
            goldText.text = "" + entity.gold;
        }
    }

    private void UpdateExperience(Entity entity, int amount)
    {
        if (this.entity == entity)
        {
            experienceSlider.value = entity.experience;
            experienceText.text = entity.experience + " / 10 XP";

        }
    }

    private void UpdateLevel(Entity entity, int amount)
    {
        if (this.enabled == entity)
        {
            levelText.text = "Lv. " + entity.level;
        }
    }

    private void UpdateFloor(Room room)
    {
        floorNumberText.text = DataManager.instance.GetRoomDescription();
    }
}
