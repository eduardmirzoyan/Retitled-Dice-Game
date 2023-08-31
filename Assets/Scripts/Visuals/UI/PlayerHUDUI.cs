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
    [SerializeField] private HealthbarUI healthbarUI;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private TextMeshProUGUI floorNumberText;

    [Header("Data")]
    [SerializeField] private Entity entity;

    private List<GameObject> heartObjects;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        heartObjects = new List<GameObject>();
    }

    private void Start()
    {
        // Sub
        GameEvents.instance.onEntitySpawn += InitializePlayer;
        GameEvents.instance.onEnterFloor += UpdateFloor;
        GameEvents.instance.onEntityGoldChange += UpdateGold;
        GameEvents.instance.onEntityGainExperience += UpdateExperience;
        GameEvents.instance.onEntityGainLevel += UpdateLevel;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEntitySpawn -= InitializePlayer;
        GameEvents.instance.onEnterFloor -= UpdateFloor;
        GameEvents.instance.onEntityGoldChange -= UpdateGold;
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

            // Set healthbar
            healthbarUI.Initialize(entity);

            // Update visuals
            UpdateIcon();
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

    private void UpdateGold(Entity entity, int amount)
    {
        // If non-zero gold was gained
        if (this.entity == entity)
        {
            goldText.text = entity.gold + "<sprite name=\"Gold\">";
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

    private void UpdateFloor(Player player)
    {
        floorNumberText.text = DataManager.instance.GetRoomDescription();
    }
}
