using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public class EntityInspectUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup windowCanvasGroup;
    [SerializeField] private CanvasGroup containerCanvasGroup;
    [SerializeField] private TextMeshProUGUI entityName;
    [SerializeField] private Image entityIcon;
    [SerializeField] private HealthbarUI healthbarUI;

    [Header("Data")]
    [SerializeField] private Entity entity;

    [Header("Settings")]
    [SerializeField] private float fadePercent;


    private void Start()
    {
        // Sub
        GameEvents.instance.onEntityInspect += InspectEntity;
        GameEvents.instance.onEntityDespawn += Uninspect;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEntityInspect -= InspectEntity;
        GameEvents.instance.onEntityDespawn -= Uninspect;
    }

    private void Uninspect(Entity entity)
    {
        // If you are inspecting this entity, stop
        if (this.entity == entity)
        {
            Disable();
        }
    }

    private void InspectEntity(Entity entity, Action _, List<Vector3Int> __)
    {
        // If entity is different, you need to make some changes
        if (this.entity != entity)
        {
            if (this.entity != null)
            {
                Disable();
            }

            if (entity != null)
            {
                Enable(entity);
            }
        }

        // Update field
        this.entity = entity;
    }

    private void Enable(Entity entity)
    {
        // Show visuals
        containerCanvasGroup.alpha = 1f;
        containerCanvasGroup.interactable = true;
        containerCanvasGroup.blocksRaycasts = true;

        // Update name
        entityName.text = entity.name;

        // Update icon
        entityIcon.sprite = entity.modelSprite;

        // Intialize healthbar
        healthbarUI.Initialize(entity);

        // Focus
        windowCanvasGroup.alpha = 1f;
    }

    private void Disable()
    {
        // Hide visuals
        containerCanvasGroup.alpha = 0;
        containerCanvasGroup.interactable = false;
        containerCanvasGroup.blocksRaycasts = false;

        // Remove any visuals
        healthbarUI.Uninitialize();

        // Fade
        windowCanvasGroup.alpha = fadePercent;
    }
}
