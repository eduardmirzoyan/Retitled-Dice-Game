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
    [SerializeField] private HorizontalLayoutGroup actionsLayoutGroup;

    [Header("Data")]
    [SerializeField] private GameObject actionUIPrefab;
    [SerializeField] private Entity entity;

    private List<GameObject> heartObjects;
    private List<ActionUI> actionUIs;

    private void Awake()
    {
        heartObjects = new List<GameObject>();
        actionUIs = new List<ActionUI>();
    }

    private void Start()
    {
        // Sub
        GameEvents.instance.onEntityInspect += UpdateInspect;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEntityInspect -= UpdateInspect;
    }

    public void UpdateInspect(Entity entity)
    {
        // If entity is different, you need to make some change
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
                DestroyActions();
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

                // Display all of the entity's actions
                foreach (var action in entity.GetActions())
                {
                    // Instaniate as child
                    var actionUI = Instantiate(actionUIPrefab, actionsLayoutGroup.transform).GetComponent<ActionUI>();
                    // Initialize in inspect mode
                    actionUI.Initialize(action, ActionMode.Inspect, entity);
                    // Save
                    actionUIs.Add(actionUI);
                }
            }
        }

        // Update field
        this.entity = entity;
    }

    private void DestroyActions()
    {
        foreach (var actionUI in actionUIs)
        {
            // Un-init
            actionUI.Uninitialize();
            // Destroy
            Destroy(actionUI.gameObject);
        }

        // Clear list
        actionUIs.Clear();
    }
}
