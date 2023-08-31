using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    // Game state
    public event Action<Player> onEnterFloor;
    public event Action<Room> onGenerateFloor;
    public event Action<Entity> onTurnStart;
    public event Action<Entity> onTurnEnd;
    public event System.Action onGameWin;
    public event System.Action onGameLose;
    public event System.Action onCombatEnter;
    public event System.Action onCombatExit;

    // Action based
    public event Action<Entity, Action> onActionSelect;
    public event Action<Entity, Action, Vector3Int> onLocationSelect;
    public event Action<Entity, Action, Vector3Int> onActionConfirm;
    public event Action<Entity, Action, Vector3Int, Room> onActionPerformStart;
    public event Action<Entity, Action, Vector3Int, Room> onActionPerformEnd;

    // Dice based
    public event Action<Die> onDieRoll;
    public event Action<Die> onDieExhaust;
    public event Action<Die> onDieReplenish;

    // Visuals
    public event Action<Entity> onEntitySpawn;
    public event Action<Entity> onEntityDespawn;
    public event Action<Entity> onEntityMoveStart;
    public event Action<Entity> onEntityMove;
    public event Action<Entity> onEntityMoveStop;
    public event Action<Entity, Vector3, Weapon> onEntityDrawWeapon;
    public event Action<Entity, Weapon> onEntitySheatheWeapon;
    public event Action<Entity, Weapon> onEntityUseWeapon;
    public event Action<Entity> onEntityWarp;
    public event Action<Entity> onEntityJump;
    public event Action<Action, Vector3Int> onActionThreatenLocation;
    public event Action<Action, Vector3Int> onActionUnthreatenLocation;

    // Stat changes
    public event Action<Entity, int> onEntityTakeDamage;
    public event Action<Entity, int> onEntityGainExperience;
    public event Action<Entity, int> onEntityGainLevel;
    public event Action<Entity, int> onEntityGoldChange;

    // Common Events
    public event Action<PickUpType, Vector3Int> onPickupSpawn;
    public event Action<Vector3Int> onPickupDespawn;
    public event System.Action onLockExit;
    public event System.Action onUnlockExit;

    // UI Based
    public event Action<Entity, Inventory> onOpenShop;
    public event Action<Inventory> onCloseShop;
    public event Action<ItemUI, ItemSlotUI> onItemInsert;
    public event Action<Entity, Weapon, int> onEquipWeapon;
    public event Action<Entity, Weapon, int> onUnequipWeapon;
    public event Action<Entity> onEntityInspect;
    public event Action<List<Vector3Int>> onThreatsInspect;

    public static GameEvents instance;
    private void Awake()
    {
        // Singleton Logic
        if (GameEvents.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void TriggerGenerateFloor(Room room)
    {
        if (onGenerateFloor != null)
        {
            onGenerateFloor(room);
        }
    }

    public void TriggerOnEnterFloor(Player player)
    {
        if (onEnterFloor != null)
        {
            onEnterFloor(player);
        }
    }

    public void TriggerOnEntitySpawn(Entity entity)
    {
        if (onEntitySpawn != null)
        {
            onEntitySpawn(entity);
        }
    }

    public void TriggerOnEntityDespawn(Entity entity)
    {
        if (onEntityDespawn != null)
        {
            onEntityDespawn(entity);
        }
    }

    public void TriggerOnTurnStart(Entity entity)
    {
        if (onTurnStart != null)
        {
            onTurnStart(entity);
        }
    }

    public void TriggerOnTurnEnd(Entity entity)
    {
        if (onTurnEnd != null)
        {
            onTurnEnd(entity);
        }
    }

    public void TriggerOnGameLose()
    {
        if (onGameLose != null)
        {
            onGameLose();
        }
    }

    public void TriggerOnGameWin()
    {
        if (onGameWin != null)
        {
            onGameWin();
        }
    }

    public void TriggerOnCombatEnter()
    {
        if (onCombatEnter != null)
        {
            onCombatEnter();
        }
    }

    public void TriggerOnCombatExit()
    {
        if (onCombatExit != null)
        {
            onCombatExit();
        }
    }

    public void TriggerOnActionSelect(Entity entity, Action action)
    {
        if (onActionSelect != null)
        {
            onActionSelect(entity, action);
        }
    }
    public void TriggerOnLocationSelect(Entity entity, Action action, Vector3Int location)
    {
        if (onLocationSelect != null)
        {
            onLocationSelect(entity, action, location);
        }
    }

    public void TriggerOnActionConfirm(Entity entity, Action action, Vector3Int location)
    {
        if (onActionConfirm != null)
        {
            onActionConfirm(entity, action, location);
        }
    }

    public void TriggerOnActionPerformStart(Entity entity, Action action, Vector3Int location, Room room)
    {
        if (onActionPerformStart != null)
        {
            onActionPerformStart(entity, action, location, room);
        }
    }

    public void TriggerOnActionPerformEnd(Entity entity, Action action, Vector3Int location, Room room)
    {
        if (onActionPerformEnd != null)
        {
            onActionPerformEnd(entity, action, location, room);
        }
    }

    public void TriggerOnEntityStartMove(Entity entity)
    {
        if (onEntityMoveStart != null)
        {
            onEntityMoveStart(entity);
        }
    }

    public void TriggerOnEntityMove(Entity entity)
    {
        if (onEntityMove != null)
        {
            onEntityMove(entity);
        }
    }

    public void TriggerOnEntityStopMove(Entity entity)
    {
        if (onEntityMoveStop != null)
        {
            onEntityMoveStop(entity);
        }
    }

    public void TriggerOnEnityWarp(Entity entity)
    {
        if (onEntityWarp != null)
        {
            onEntityWarp(entity);
        }
    }

    public void TriggerOnEntityJump(Entity entity)
    {
        if (onEntityJump != null)
        {
            onEntityJump(entity);
        }
    }

    public void TriggerOnDieRoll(Die die)
    {
        if (onDieRoll != null)
        {
            onDieRoll(die);
        }
    }

    public void TriggerOnDieExhaust(Die die)
    {
        if (onDieExhaust != null)
        {
            onDieExhaust(die);
        }
    }

    public void TriggerOnDieReplenish(Die die)
    {
        if (onDieReplenish != null)
        {
            onDieReplenish(die);
        }
    }

    public void TriggerOnEntityTakeDamage(Entity entity, int damage)
    {
        if (onEntityTakeDamage != null)
        {
            onEntityTakeDamage(entity, damage);
        }
    }

    public void TriggerOnEntityUseWeapon(Entity entity, Weapon weapon)
    {
        if (onEntityUseWeapon != null)
        {
            onEntityUseWeapon(entity, weapon);
        }
    }

    public void TriggerOnEntityDrawWeapon(Entity entity, Vector3 direction, Weapon weapon)
    {
        if (onEntityDrawWeapon != null)
        {
            onEntityDrawWeapon(entity, direction, weapon);
        }
    }

    public void TriggerOnEntitySheatheWeapon(Entity entity, Weapon weapon)
    {
        if (onEntitySheatheWeapon != null)
        {
            onEntitySheatheWeapon(entity, weapon);
        }
    }

    public void TriggerOnPickupSpawn(PickUpType pickUpType, Vector3Int location)
    {
        if (onPickupSpawn != null)
        {
            onPickupSpawn(pickUpType, location);
        }
    }

    public void TriggerOnPickupDespawn(Vector3Int location)
    {
        if (onPickupDespawn != null)
        {
            onPickupDespawn(location);
        }
    }

    public void TriggerOnUnlockExit()
    {
        if (onUnlockExit != null)
        {
            onUnlockExit();
        }
    }

    public void TriggerOnLockExit()
    {
        if (onLockExit != null)
        {
            onLockExit();
        }
    }

    public void TriggerOnGainExperience(Entity entity, int amount)
    {
        if (onEntityGainExperience != null)
        {
            onEntityGainExperience(entity, amount);
        }
    }

    public void TriggerOnGainLevel(Entity entity, int amount)
    {
        if (onEntityGainLevel != null)
        {
            onEntityGainLevel(entity, amount);
        }
    }

    public void TriggerOnGoldChange(Entity entity, int amount)
    {
        if (onEntityGoldChange != null)
        {
            onEntityGoldChange(entity, amount);
        }
    }

    public void TriggerOnItemInsert(ItemUI itemUI, ItemSlotUI itemSlotUI)
    {
        if (onItemInsert != null)
        {
            onItemInsert(itemUI, itemSlotUI);
        }
    }

    public void TriggerOnEquipWeapon(Entity entity, Weapon weapon, int index)
    {
        if (onEquipWeapon != null)
        {
            onEquipWeapon(entity, weapon, index);
        }
    }

    public void TriggerOnUnequipWeapon(Entity entity, Weapon weapon, int index)
    {
        if (onUnequipWeapon != null)
        {
            onUnequipWeapon(entity, weapon, index);
        }
    }

    public void TriggerOnEntityInspect(Entity entity)
    {
        if (onEntityInspect != null)
        {
            onEntityInspect(entity);
        }
    }

    public void TriggerOnThreatsInspect(List<Vector3Int> locations)
    {
        if (onThreatsInspect != null)
        {
            onThreatsInspect(locations);
        }
    }

    public void TriggerOnActionThreatenLocation(Action action, Vector3Int location)
    {
        if (onActionThreatenLocation != null)
        {
            onActionThreatenLocation(action, location);
        }
    }

    public void TriggerOnActionUnthreatenLocation(Action action, Vector3Int location)
    {
        if (onActionUnthreatenLocation != null)
        {
            onActionUnthreatenLocation(action, location);
        }
    }

    public void TriggerOnOpenShop(Entity entity, Inventory inventory)
    {
        if (onOpenShop != null)
        {
            onOpenShop(entity, inventory);
        }
    }

    public void TriggerOnCloseShop(Inventory inventory)
    {
        if (onCloseShop != null)
        {
            onCloseShop(inventory);
        }
    }
}
