using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionInfo
{
    public float waitTime;

    public ActionInfo()
    {
        this.waitTime = 0f;
    }

    public ActionInfo(float waitTime)
    {
        this.waitTime = waitTime;
    }
}

public class GameEvents : MonoBehaviour
{
    public event Action<Room> onEnterFloor;
    public event Action<Room> onExitFloor;

    // Game state
    public event Action<Entity> onTurnStart;
    public event Action<Entity> onTurnEnd;

    // Action based
    public event Action<Entity, Action> onActionSelect;
    public event Action<Entity, Action, Vector3Int> onLocationSelect;
    public event Action<Entity, Action, Vector3Int, Room> onActionPerformStart;
    public event Action<Entity, Action, Vector3Int, Room> onActionPerformEnd;


    // Dice based
    public event Action<Die> onDieRoll;
    public event Action<Die> onDieExhaust;
    public event Action<Die> onDieReplenish;

    // Visuals
    public event Action<Entity> onEntitySpawn;
    public event Action<Entity> onEnityDespawn;
    public event Action<Entity> onEntityMoveStart;
    public event Action<Entity> onEntityMove;
    public event Action<Entity> onEntityMoveStop;
    public event Action<Entity, Vector3, Weapon> onEntityDrawWeapon;
    public event Action<Entity, Weapon> onEntitySheatheWeapon;
    public event Action<Entity, Weapon> onEntityMeleeAttack;
    public event Action<Entity, Vector3Int, Weapon> onEntityRangedAttack;
    public event Action<Entity, Vector3Int, Weapon, ActionInfo> onEntityRangedAttackTimed;
    public event Action<Entity> onEntityWarp;

    // Projectile visuals
    public event Action<Projectil> onProjectileSpawn;
    public event Action<Projectil> onProjectileMove;
    public event Action<Projectil> onProjectileDespawn;

    // Stat changes
    public event Action<Entity, int> onEntityTakeDamage;
    public event Action<Entity, int> onEntityGainExperience;
    public event Action<Entity, int> onEntityGainLevel;
    public event Action<Entity, int> onEntityGainGold;

    public event Action<Entity, int> onPickup;
    public event Action<int> onUseKey;

    public event Action<Inventory> onOpenShop;
    public event System.Action onCloseShop;

    public event Action<ItemUI, ItemSlotUI> onItemInsert;
    public event Action<Entity, Weapon> onWeaponEquip;
    public event Action<Inventory, bool> onToggleInventory;
    public event Action<Entity> onEntityInspect;
    public event Action<Entity, Action> onInspectAction;

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

    public void TriggerOnOpenShop(Inventory inventory)
    {
        if (onOpenShop != null)
        {
            onOpenShop(inventory);
        }
    }

    public void TriggerOnEnterFloor(Room room)
    {
        if (onEnterFloor != null)
        {
            onEnterFloor(room);
        }
    }

    public void TriggerOnExitFloor(Room room)
    {
        if (onExitFloor != null)
        {
            onExitFloor(room);
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
        if (onEnityDespawn != null)
        {
            onEnityDespawn(entity);
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

    public void TriggerOnEntityMeleeAttack(Entity entity, Weapon weapon)
    {
        if (onEntityMeleeAttack != null)
        {
            onEntityMeleeAttack(entity, weapon);
        }
    }

    public IEnumerator TriggerOnEntityRangedAttack(Entity entity, Vector3Int targetLocation, Weapon weapon, ActionInfo info)
    {
        // If there are any subscribers
        if (onEntityRangedAttackTimed != null)
        {
            // Invoke all the subscribers
            foreach (var invocation in onEntityRangedAttackTimed.GetInvocationList())
            {
                // Call invokation and update info values
                invocation.DynamicInvoke(entity, targetLocation, weapon, info);
                // Wait an amount of time
                yield return new WaitForSeconds(info.waitTime);
                // Reset wait time
                info.waitTime = 0f;
            }
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

    public void TriggerOnPickup(Entity entity, int index)
    {
        if (onPickup != null)
        {
            onPickup(entity, index);
        }
    }

    public void TriggerOnUseKey(int value)
    {
        if (onUseKey != null)
        {
            onUseKey(value);
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

    public void TriggerOnGainGold(Entity entity, int amount)
    {
        if (onEntityGainGold != null)
        {
            onEntityGainGold(entity, amount);
        }
    }

    public void TriggerOnItemInsert(ItemUI itemUI, ItemSlotUI itemSlotUI)
    {
        if (onItemInsert != null)
        {
            onItemInsert(itemUI, itemSlotUI);
        }
    }

    public void TriggerOnWeaponEquip(Entity entity, Weapon weapon)
    {
        if (onWeaponEquip != null)
        {
            onWeaponEquip(entity, weapon);
        }
    }

    public void TriggerOnToggleInventory(Inventory inventory, bool state)
    {
        if (onToggleInventory != null)
        {
            onToggleInventory(inventory, state);
        }
    }

    public void TriggerOnEntityInspect(Entity entity)
    {
        if (onEntityInspect != null)
        {
            onEntityInspect(entity);
        }
    }

    public void TriggerOnInspectAction(Entity entity, Action action, Room room)
    {
        if (onInspectAction != null)
        {
            onInspectAction(entity, action);
        }
    }

    public void TriggerOnProjectileSpawn(Projectil projectile)
    {
        if (onProjectileSpawn != null)
        {
            onProjectileSpawn(projectile);
        }
    }

    public void TriggerOnProjectileDespawn(Projectil projectile)
    {
        if (onProjectileDespawn != null)
        {
            onProjectileDespawn(projectile);
        }
    }

    public void TriggerOnProjectileMoveStart(Projectil projectile)
    {
        // if (onProjectileMoveStart != null)
        // {
        //     onProjectileMoveStart(projectile);
        // }
    }

    public void TriggerOnProjectileMove(Projectil projectile)
    {
        if (onProjectileMove != null)
        {
            onProjectileMove(projectile);
        }
    }

    public void TriggerOnProjectileMoveStop(Projectil projectile)
    {
        // if (onProjectileMoveStart != null)
        // {
        //     onProjectileMoveStart(projectile);
        // }
    }
}
