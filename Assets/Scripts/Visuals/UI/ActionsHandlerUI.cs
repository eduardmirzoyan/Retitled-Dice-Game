using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsHandlerUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private GameObject actionUIPrefab;

    [Header("Dynamic Data")]
    [SerializeField] private List<ActionUI> actionUIs;
    [SerializeField] private Entity entity;

    private void Start()
    {
        // Initialize list
        actionUIs = new List<ActionUI>();

        // Sub
        GameEvents.instance.onEnterFloor += Initialize;
        GameEvents.instance.onEquipWeapon += UpdateActions;
        GameEvents.instance.onUnequipWeapon += UpdateActions;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= Initialize;
        GameEvents.instance.onEquipWeapon += UpdateActions;
        GameEvents.instance.onUnequipWeapon += UpdateActions;
    }

    private void Initialize(Player player)
    {
        if (player != null)
        {
            // Save and create actions
            this.entity = player;
            CreateActions(player);
        }
        else
        {
            // Clear actions
            DestroyActions();
        }
    }

    private void UpdateActions(Entity entity, Weapon weapon, int index)
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
            var actionUI = Instantiate(actionUIPrefab, gridLayoutGroup.transform).GetComponent<ActionUI>();
            // Initialize
            actionUI.Initialize(action, ActionMode.Interact);
            // Save
            actionUIs.Add(actionUI);
        }
    }
}
