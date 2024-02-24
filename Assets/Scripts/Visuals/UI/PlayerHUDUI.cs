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
    [SerializeField] private TextMeshProUGUI floorNumberText;

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

    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEntitySpawn -= InitializePlayer;
        GameEvents.instance.onEnterFloor -= UpdateFloor;

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

    private void UpdateFloor(Player player)
    {
        floorNumberText.text = DataManager.instance.GetRoomDescription();
    }
}
