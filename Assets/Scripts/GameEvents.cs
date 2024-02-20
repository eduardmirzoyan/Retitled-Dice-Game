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
    public event Action<Action> onActionDisable;
    public event Action<Action> onActionEnable;
    public event Action<Entity, Action> onActionSelect;
    public event Action<Entity, Action> onActionDeselect;
    public event Action<Entity, Action, Vector3Int> onLocationSelect;
    public event Action<Entity, Action, Vector3Int> onActionConfirm;
    public event Action<Entity, Action, Vector3Int, Room> onActionPerformStart;
    public event Action<Entity, Action, Vector3Int, Room> onActionPerformEnd;

    // Dice based
    public event Action<Die> onDieLoop;
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
    public event Action<Action, List<Vector3Int>> onActionThreatenLocations;
    public event Action<Action, List<Vector3Int>> onActionUnthreatenLocations;
    public event Action<Entity> onEntityInDanger;
    public event Action<Entity> onEntityOutDanger;

    // Events
    public event Action<Entity, Weapon, Entity> onEntityKillEntity;

    // Stat changes
    public event Action<Entity, int> onEntityTakeDamage;
    public event Action<Entity, int> onEntityGoldChange;

    // Common Events
    public event Action<PickUpType, Vector3Int> onPickupSpawn;
    public event Action<Vector3Int> onPickupDespawn;
    public event System.Action onLockExit;
    public event System.Action onUnlockExit;

    // UI Based
    public event Action<Entity, Inventory> onOpenShop;
    public event Action<Inventory> onCloseShop;
    public event Action<Entity> onOpenBlacksmith;
    public event Action<Entity> onCloseBlacksmith;
    public event Action<Weapon> onInsertBlacksmith;
    public event Action<Weapon> onRemoveBlacksmith;
    public event Action<ItemUI, ItemSlotUI> onItemInsert;
    public event Action<Entity, Weapon, int> onEquipWeapon;
    public event Action<Entity, Weapon, int> onUnequipWeapon;
    public event Action<Entity, Consumable> onEntityUseConsumable;

    public event Action<Entity, List<Vector3Int>> onEntityInspect;

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
        onGenerateFloor?.Invoke(room);
    }

    public void TriggerOnEnterFloor(Player player)
    {
        onEnterFloor?.Invoke(player);
    }

    public void TriggerOnEntitySpawn(Entity entity)
    {
        onEntitySpawn?.Invoke(entity);
    }

    public void TriggerOnEntityDespawn(Entity entity)
    {
        onEntityDespawn?.Invoke(entity);
    }

    public void TriggerOnTurnStart(Entity entity)
    {
        onTurnStart?.Invoke(entity);
    }

    public void TriggerOnTurnEnd(Entity entity)
    {
        onTurnEnd?.Invoke(entity);
    }

    public void TriggerOnGameLose()
    {
        onGameLose?.Invoke();
    }

    public void TriggerOnGameWin()
    {
        onGameWin?.Invoke();
    }

    public void TriggerOnCombatEnter()
    {
        onCombatEnter?.Invoke();
    }

    public void TriggerOnCombatExit()
    {
        onCombatExit?.Invoke();
    }

    public void TriggerOnActionEnable(Action action)
    {
        onActionEnable?.Invoke(action);
    }

    public void TriggerOnActionDisable(Action action)
    {
        onActionDisable?.Invoke(action);
    }

    public void TriggerOnActionSelect(Entity entity, Action action)
    {
        onActionSelect?.Invoke(entity, action);
    }

    public void TriggerOnActionDeselect(Entity entity, Action action)
    {
        onActionDeselect?.Invoke(entity, action);
    }

    public void TriggerOnLocationSelect(Entity entity, Action action, Vector3Int location)
    {
        onLocationSelect?.Invoke(entity, action, location);
    }

    public void TriggerOnActionConfirm(Entity entity, Action action, Vector3Int location)
    {
        onActionConfirm?.Invoke(entity, action, location);
    }

    public void TriggerOnActionPerformStart(Entity entity, Action action, Vector3Int location, Room room)
    {
        onActionPerformStart?.Invoke(entity, action, location, room);
    }

    public void TriggerOnActionPerformEnd(Entity entity, Action action, Vector3Int location, Room room)
    {
        onActionPerformEnd?.Invoke(entity, action, location, room);
    }

    public void TriggerOnEntityStartMove(Entity entity)
    {
        onEntityMoveStart?.Invoke(entity);
    }

    public void TriggerOnEntityMove(Entity entity)
    {
        onEntityMove?.Invoke(entity);
    }

    public void TriggerOnEntityStopMove(Entity entity)
    {
        onEntityMoveStop?.Invoke(entity);
    }

    public void TriggerOnEnityWarp(Entity entity)
    {
        onEntityWarp?.Invoke(entity);
    }

    public void TriggerOnEntityJump(Entity entity)
    {
        onEntityJump?.Invoke(entity);
    }

    public void TriggerOnDieLoop(Die die)
    {
        onDieLoop?.Invoke(die);
    }

    public void TriggerOnDieRoll(Die die)
    {
        onDieRoll?.Invoke(die);
    }

    public void TriggerOnDieExhaust(Die die)
    {
        onDieExhaust?.Invoke(die);
    }

    public void TriggerOnDieReplenish(Die die)
    {
        onDieReplenish?.Invoke(die);
    }

    public void TriggerOnEntityTakeDamage(Entity entity, int damage)
    {
        onEntityTakeDamage?.Invoke(entity, damage);
    }

    public void TriggerOnEntityUseWeapon(Entity entity, Weapon weapon)
    {
        onEntityUseWeapon?.Invoke(entity, weapon);
    }

    public void TriggerOnEntityDrawWeapon(Entity entity, Vector3 direction, Weapon weapon)
    {
        onEntityDrawWeapon?.Invoke(entity, direction, weapon);
    }

    public void TriggerOnEntitySheatheWeapon(Entity entity, Weapon weapon)
    {
        onEntitySheatheWeapon?.Invoke(entity, weapon);
    }

    public void TriggerOnPickupSpawn(PickUpType pickUpType, Vector3Int location)
    {
        onPickupSpawn?.Invoke(pickUpType, location);
    }

    public void TriggerOnPickupDespawn(Vector3Int location)
    {
        onPickupDespawn?.Invoke(location);
    }

    public void TriggerOnUnlockExit()
    {
        onUnlockExit?.Invoke();
    }

    public void TriggerOnLockExit()
    {
        onLockExit?.Invoke();
    }

    public void TriggerOnGoldChange(Entity entity, int amount)
    {
        onEntityGoldChange?.Invoke(entity, amount);
    }

    public void TriggerOnItemInsert(ItemUI itemUI, ItemSlotUI itemSlotUI)
    {
        onItemInsert?.Invoke(itemUI, itemSlotUI);
    }

    public void TriggerOnEquipWeapon(Entity entity, Weapon weapon, int index)
    {
        onEquipWeapon?.Invoke(entity, weapon, index);
    }

    public void TriggerOnUnequipWeapon(Entity entity, Weapon weapon, int index)
    {
        onUnequipWeapon?.Invoke(entity, weapon, index);
    }

    public void TriggerOnEntityUseConsumable(Entity entity, Consumable consumable)
    {
        onEntityUseConsumable?.Invoke(entity, consumable);
    }

    public void TriggerOnEntityInspect(Entity entity, List<Vector3Int> locations)
    {
        onEntityInspect?.Invoke(entity, locations);
    }

    public void TriggerOnActionThreatenLocation(Action action, List<Vector3Int> locations)
    {
        onActionThreatenLocations?.Invoke(action, locations);
    }

    public void TriggerOnActionUnthreatenLocation(Action action, List<Vector3Int> locations)
    {
        onActionUnthreatenLocations?.Invoke(action, locations);
    }

    public void TriggerOnEntityInDanger(Entity entity)
    {
        onEntityInDanger?.Invoke(entity);
    }

    public void TriggerOnEntityOutDanger(Entity entity)
    {
        onEntityOutDanger?.Invoke(entity);
    }

    public void TriggerOnEntityKillEntity(Entity killer, Weapon weapon, Entity victim)
    {
        onEntityKillEntity?.Invoke(killer, weapon, victim);
    }

    public void TriggerOnOpenShop(Entity entity, Inventory inventory)
    {
        onOpenShop?.Invoke(entity, inventory);
    }

    public void TriggerOnCloseShop(Inventory inventory)
    {
        onCloseShop?.Invoke(inventory);
    }

    public void TriggerOnOpenBlacksmith(Entity entity)
    {
        onOpenBlacksmith?.Invoke(entity);
    }

    public void TriggerOnCloseBlacksmith(Entity entity)
    {
        onCloseBlacksmith?.Invoke(entity);
    }

    public void TriggerOnInsertBlacksmith(Weapon weapon)
    {
        onInsertBlacksmith?.Invoke(weapon);
    }

    public void TriggerOnRemoveBlacksmith(Weapon weapon)
    {
        onRemoveBlacksmith?.Invoke(weapon);
    }
}
