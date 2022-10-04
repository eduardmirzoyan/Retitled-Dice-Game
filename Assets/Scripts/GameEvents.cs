using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public event Action<Room> onEnterFloor;
    public event Action<Entity> onSpawnEntity;
    public event Action<Entity> onRemoveEntity;
    public event Action<Room> onExitFloor;

    // Game state
    public event Action<Entity> onTurnStart;
    public event Action<Entity> onTurnEnd;

    // Action based
    public event Action<Entity, Action, Room> onActionSelect;
    public event Action<Entity, Vector3Int> onLocationSelect;
    public event Action<Entity, Action, Vector3Int, Room> onActionPerformStart;
    public event Action<Entity, Action, Vector3Int, Room> onActionPerformEnd;


    // Dice based
    public event Action<Die> onDieRoll;
    public event Action<Die> onDieExhaust;
    public event Action<Die> onDieReplenish;

    // Visuals
    public event Action<Entity, bool> onEntityMove;
    public event Action<Entity, bool> onEntityReadyWeapon;
    public event Action<Entity> onEntityMeleeAttack;
    public event Action<Entity> onEntityWarp;

    // Stat changes
    public event Action<Entity, int> onEntityTakeDamage;
    public event Action<Entity, int> onEntityGainExperience;
    public event Action<Entity, int> onEntityGainLevel;
    public event Action<Entity, int> onEntityGainGold;

    public event Action<Entity, int> onPickup;
    public event Action<int> onUseKey;

    public event Action<bool> onOpenShop;

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

    public void TriggerOnOpenShop(bool state)
    {
        if (onOpenShop != null)
        {
            onOpenShop(state);
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

    public void TriggerOnSpawnEnity(Entity entity)
    {
        if (onSpawnEntity != null)
        {
            onSpawnEntity(entity);
        }
    }

    public void TriggerOnRemoveEnity(Entity entity)
    {
        if (onRemoveEntity != null)
        {
            onRemoveEntity(entity);
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

    public void TriggerOnActionSelect(Entity entity, Action action, Room room)
    {
        if (onActionSelect != null)
        {
            onActionSelect(entity, action, room);
        }
    }

    public void TriggerOnLocationSelect(Entity entity, Vector3Int location)
    {
        if (onLocationSelect != null)
        {
            onLocationSelect(entity, location);
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

    public void TriggerOnEntityMove(Entity entity, bool state)
    {
        if (onEntityMove != null)
        {
            onEntityMove(entity, state);
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

    public void TriggerOnEntityMeleeAttack(Entity entity)
    {
        if (onEntityMeleeAttack != null)
        {
            onEntityMeleeAttack(entity);
        }
    }

    public void TriggerOnEntityReadyWeapon(Entity entity, bool state)
    {
        if (onEntityReadyWeapon != null)
        {
            onEntityReadyWeapon(entity, state);
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
}
