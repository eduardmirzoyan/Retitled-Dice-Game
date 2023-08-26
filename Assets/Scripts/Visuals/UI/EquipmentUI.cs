using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ItemSlotUI primaryWeaponSlotUI;
    [SerializeField] private ItemSlotUI secondaryWeaponSlotUI;

    [Header("Data")]
    [SerializeField] private Player player;
    [SerializeField] private Room room;
    [SerializeField] private bool isLocked;

    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += Initialize;
        GameEvents.instance.onItemInsert += EquipWeapon;
        GameEvents.instance.onEntityDespawn += CheckUnlock;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= Initialize;
        GameEvents.instance.onItemInsert -= EquipWeapon;
        GameEvents.instance.onEntityDespawn -= CheckUnlock;
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

        // Check clock state
        CheckUnlock(player);
    }

    private void EquipWeapon(ItemUI itemUI, ItemSlotUI itemSlotUI)
    {
        // Equip mainhand
        if (itemSlotUI == primaryWeaponSlotUI)
        {
            if (itemUI != null) player.EquipMainhand((Weapon)itemUI.GetItem());
            else player.EquipMainhand(null);
        }
        // Equip offhand
        else if (itemSlotUI == secondaryWeaponSlotUI)
        {
            if (itemUI != null) player.EquipOffhand((Weapon)itemUI.GetItem());
            else player.EquipOffhand(null);
        }
    }

    private void CheckUnlock(Entity entity)
    {
        // When an enemy dies, check if there are any hostiles left
        if (isLocked && !room.HasHostileEntities())
        {
            // Unlock
            Unlock();
        }
        else if (!isLocked && room.HasHostileEntities())
        {
            // Lock
            Lock();
        }
    }

    private void Lock()
    {
        // Lock slots
        primaryWeaponSlotUI.Lock();

        secondaryWeaponSlotUI.Lock();

        isLocked = true;
    }

    private void Unlock()
    {
        // Unlock slots
        primaryWeaponSlotUI.Unlock();
        secondaryWeaponSlotUI.Unlock();

        isLocked = false;
    }

}
