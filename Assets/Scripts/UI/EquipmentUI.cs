using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ItemSlotUI primaryWeaponSlotUI;
    [SerializeField] private ItemSlotUI secondaryWeaponSlotUI;
    [SerializeField] private Image lockImage;

    [Header("Data")]
    [SerializeField] private Player player;

    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += Initialize;
        GameEvents.instance.onItemInsert += EquipWeapon;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= Initialize;
        GameEvents.instance.onItemInsert -= EquipWeapon;
    }

    private void Initialize(Room room)
    {
        // Save
        this.player = room.player;

        // Initialize player's equipment
        if (player.primaryWeapon != null)
        {
            // Spawn itemUI here
            primaryWeaponSlotUI.CreateItem(room.player.primaryWeapon);
        }

        if (player.secondaryWeapon != null)
        {
            // Spawn itemUI here
            secondaryWeaponSlotUI.CreateItem(room.player.secondaryWeapon);
        }
    }

    private void EquipWeapon(ItemUI itemUI, ItemSlotUI itemSlotUI)
    {
        // Equip primary
        if (itemSlotUI == primaryWeaponSlotUI)
        {
            if (itemUI != null) player.EquipPrimary((Weapon)itemUI.GetItem());
            else player.EquipPrimary(null);
        }
        // Equip primary
        else if (itemSlotUI == secondaryWeaponSlotUI)
        {
            if (itemUI != null) player.EquipSecondary((Weapon)itemUI.GetItem());
            else player.EquipSecondary(null);
        }
    }

}
