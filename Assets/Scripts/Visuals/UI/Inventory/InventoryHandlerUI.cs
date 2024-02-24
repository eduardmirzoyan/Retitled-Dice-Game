using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryHandlerUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private TextMeshProUGUI goldLabel;

    [Header("Data")]
    [SerializeField] private GameObject itemSlotPrefab;

    [Header("Debug")]
    [SerializeField] private Entity entity;

    private void Awake()
    {
        gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
    }

    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += Initialize;
        GameEvents.instance.onEntityGoldChange += UpdateGold;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= Initialize;
        GameEvents.instance.onEntityGoldChange -= UpdateGold;
    }

    private void Initialize(Player player)
    {
        if (player == null)
            throw new System.Exception("Player is null.");

        entity = player;
        var inventory = player.inventory;

        // Fill item slots with entity's inventory
        for (int i = 0; i < inventory.maxSize; i++)
        {
            // Create slot in grid
            var inventorySlot = Instantiate(itemSlotPrefab, gridLayoutGroup.transform).GetComponent<InventorySlotUI>();
            inventorySlot.Initialize(inventory, i);
        }

        // Update count
        UpdateGold(player, 0);
    }

    private void UpdateGold(Entity entity, int _)
    {
        // If non-zero gold was gained
        if (this.entity == entity)
        {
            goldLabel.text = entity.gold + "<sprite name=\"Gold\">";
        }
    }
}
