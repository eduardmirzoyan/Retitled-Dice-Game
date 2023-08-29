using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandlerUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private GameObject itemSlotPrefab;

    private void Awake()
    {
        gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
    }

    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += Initialize;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= Initialize;
    }

    private void Initialize(Player player)
    {
        var inventory = player.inventory;

        // Fill item slots with entity's inventory
        for (int i = 0; i < inventory.maxSize; i++)
        {
            // Create slot in grid
            var inventorySlot = Instantiate(itemSlotPrefab, gridLayoutGroup.transform).GetComponent<InventorySlotUI>();
            inventorySlot.Initialize(inventory, i);
        }
    }
}
