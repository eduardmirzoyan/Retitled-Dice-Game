using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntityInspectUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI entityName;
    [SerializeField] private Image entityIcon;
    [SerializeField] private HealthbarUI healthbarUI;

    [Header("Data")]
    [SerializeField] private Entity entity;


    private void Start()
    {
        // Sub
        GameEvents.instance.onEntityInspect += InspectEntity;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEntityInspect -= InspectEntity;
    }

    public void InspectEntity(Entity entity)
    {
        // If entity is different, you need to make some changes
        if (this.entity != entity)
        {
            if (this.entity != null)
            {
                // Hide visuals
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;

                // Remove any visuals
                healthbarUI.Uninitialize();
            }

            if (entity != null)
            {
                // Show visuals
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                // Update name
                entityName.text = entity.name;

                // Update icon
                entityIcon.sprite = entity.modelSprite;

                // Intialize healthbar
                healthbarUI.Initialize(entity);
            }
        }

        // Update field
        this.entity = entity;
    }
}
