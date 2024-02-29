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
    [SerializeField] private TextMeshProUGUI floorNumberText;
    [SerializeField] private List<EntityEnchantmentSlotUI> enchantmentUIs;

    [Header("Data")]
    [SerializeField] private Entity entity;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Start()
    {
        // Sub
        GameEvents.instance.onEntitySpawn += InitializePlayer;
        GameEvents.instance.onEnterFloor += UpdateFloor;
        GameEvents.instance.onEntityGainEnchantment += UpdateEnchantments;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEntitySpawn -= InitializePlayer;
        GameEvents.instance.onEnterFloor -= UpdateFloor;
        GameEvents.instance.onEntityGainEnchantment -= UpdateEnchantments;
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
            UpdateFloor(null);
            UpdateEnchantments(entity, null);

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

    private void UpdateFloor(Player _)
    {
        floorNumberText.text = DataManager.instance.GetRoomDescription();
    }

    private void UpdateEnchantments(Entity entity, EntityEnchantment _)
    {
        if (entity is Player)
        {
            if (entity.enchantments.Count > enchantmentUIs.Count)
                return;

            for (int i = 0; i < entity.enchantments.Count; i++)
            {
                enchantmentUIs[i].Initialize(entity.enchantments[i]);
            }
        }
    }
}
