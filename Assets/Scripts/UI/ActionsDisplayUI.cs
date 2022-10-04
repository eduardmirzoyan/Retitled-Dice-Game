using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsDisplayUI : MonoBehaviour
{
    [SerializeField] private GameObject actionUIPrefab;
    private List<ActionUI> actionUIs;

    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += DisplayPlayerActions;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= DisplayPlayerActions;
    }

    private void DisplayPlayerActions(Room dungeon)
    {
        if (dungeon != null)
        {
            // Initialize list
            actionUIs = new List<ActionUI>();

            // Display all of the player's actions
            foreach (var action in dungeon.player.innateActions)
            {
                // Instaniate as child
                var actionUI = Instantiate(actionUIPrefab, transform).GetComponent<ActionUI>();
                // Initialize
                actionUI.Initialize(action);
                // Save
                actionUIs.Add(actionUI);
            }
        }
        else
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
}
