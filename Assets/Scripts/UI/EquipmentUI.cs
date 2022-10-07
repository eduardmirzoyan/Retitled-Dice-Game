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
    [SerializeField] private Room room;
    private bool isLocked;

    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += Initialize;
        GameEvents.instance.onItemInsert += EquipWeapon;
        GameEvents.instance.onRemoveEntity += CheckUnlock;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= Initialize;
        GameEvents.instance.onItemInsert -= EquipWeapon;
        GameEvents.instance.onRemoveEntity -= CheckUnlock;
    }

    private void Initialize(Room room)
    {
        // Save
        this.player = room.player;
        this.room = room;

        // Initialize player's equipment
        if (player.mainWeapon != null)
        {
            // Spawn itemUI here
            primaryWeaponSlotUI.CreateItem(room.player.mainWeapon);
        }

        if (player.offWeapon != null)
        {
            // Spawn itemUI here
            secondaryWeaponSlotUI.CreateItem(room.player.offWeapon);
        }

        // Set locked state
        isLocked = room.enemies.Count > 0;

        // Check how many enemies are there
        if (isLocked)
        {
            Lock();
        }
        else
        {
            Unlock();
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

    private void CheckUnlock(Entity entity)
    {
        // When an enemy dies, check if there are 0 left
        if (isLocked && room.enemies.Count == 0)
        {
            // Unlock
            Unlock();
        }
        else if (!isLocked && room.enemies.Count > 0)
        {
            // Lock
            Lock();
        }
    }

    private void Lock()
    {
        // Show visuals
        lockImage.enabled = true;

        // Lock slots
        primaryWeaponSlotUI.DisableRemove();
        secondaryWeaponSlotUI.DisableRemove();
    }

    private void Unlock()
    {
        // Show visuals
        lockImage.enabled = false;

        // Lock slots
        primaryWeaponSlotUI.EnableRemove();
        secondaryWeaponSlotUI.EnableRemove();
    }

}
