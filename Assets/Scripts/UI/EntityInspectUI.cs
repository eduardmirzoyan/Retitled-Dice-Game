using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityInspectUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image entityIcon;
    [SerializeField] private HorizontalLayoutGroup heartsLayoutGroup;
    [SerializeField] private HorizontalLayoutGroup actionsLayoutGroup;

    [Header("Data")]
    [SerializeField] private GameObject actionUIPrefab;
    [SerializeField] private GameObject heartPrefab;
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
        GameEvents.instance.onInspectEntity += Initialize;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onInspectEntity -= Initialize;
    }

    public void Initialize(Entity entity)
    {
        this.entity = entity;

        // Update icon
        entityIcon.sprite = entity.modelSprite;

        // Update health
        for (int i = 0; i < entity.currentHealth; i++)
        {
            // Create heart
            heartObjects.Add(Instantiate(heartPrefab, heartsLayoutGroup.transform));
        }

        // Display all of the entity's actions
        foreach (var action in entity.GetActions())
        {
            // Instaniate as child
            var actionUI = Instantiate(actionUIPrefab, actionsLayoutGroup.transform).GetComponent<ActionUI>();
            // Initialize
            actionUI.Initialize(action);
            // Save
            actionUIs.Add(actionUI);
        }
    }
}
