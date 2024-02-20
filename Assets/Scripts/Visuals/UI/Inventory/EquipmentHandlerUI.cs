using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentHandlerUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    [SerializeField] private GameObject equipmentSlotPrefab;

    private void Awake()
    {
        horizontalLayoutGroup = GetComponentInChildren<HorizontalLayoutGroup>();
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
        // Create slots
        var mainEquipmentSlot = Instantiate(equipmentSlotPrefab, horizontalLayoutGroup.transform).GetComponent<EquipmentSlotUI>();
        mainEquipmentSlot.Initialize(player, 0);

        var offEquipmentSlot = Instantiate(equipmentSlotPrefab, horizontalLayoutGroup.transform).GetComponent<EquipmentSlotUI>();
        offEquipmentSlot.Initialize(player, 1);
    }

}
