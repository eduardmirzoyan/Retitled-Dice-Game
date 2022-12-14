using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Tile highlight -> Preview
// Arrow Indicator -> Confirmed

public class ActionsDisplayUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private GameObject actionUIPrefab;
    [SerializeField] private List<ActionUI> actionUIs;
    [SerializeField] private Entity entity;

    private void Start()
    {
        // Initialize list
        actionUIs = new List<ActionUI>();

        // Sub
        GameEvents.instance.onEnterFloor += DisplayActions;
        GameEvents.instance.onWeaponEquip += UpdateActions;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= DisplayActions;
        GameEvents.instance.onWeaponEquip -= UpdateActions;
    }

    private void DisplayActions(Room dungeon)
    {
        if (dungeon != null)
        {
            // Save and create actions
            this.entity = dungeon.player;
            CreateActions(dungeon.player);
        }
        else
        {
            // Clear actions
            DestroyActions();
        }
    }

    private void UpdateActions(Entity entity, Weapon weapon)
    {
        // If this entity weapons changed
        if (this.entity == entity)
        {
            // Clear current actions
            DestroyActions();

            // Create actions
            CreateActions(entity);
        }
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

    private void CreateActions(Entity entity) 
    {
        // Display all of the player's actions
        foreach (var action in entity.GetActions())
        {
            // Instaniate as child
            var actionUI = Instantiate(actionUIPrefab, transform).GetComponent<ActionUI>();
            // Initialize
            actionUI.Initialize(action, ActionMode.Interact);
            // Save
            actionUIs.Add(actionUI);
        }
    }
}
