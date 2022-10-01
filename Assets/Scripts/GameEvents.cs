using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public event Action<Dungeon> onEnterFloor;
    public event Action<Entity> onSpawnEntity;
    public event Action<Entity> onRemoveEntity;
    public event Action<Dungeon> onExitFloor;

    // Game state
    public event Action<Entity> onTurnStart;
    public event Action<Entity> onTurnEnd;

    // Action based
    public event Action<Entity, Action, Dungeon> onActionSelect;
    public event Action<Vector3Int> onLocationSelect;
    public event Action<Entity, Action, Vector3Int, Dungeon> onActionPerformStart;
    public event Action<Entity, Action, Vector3Int, Dungeon> onActionPerformEnd;


    // Dice based
    public event Action<Die> onDieRoll;
    public event Action<Die> onDieExhaust;
    public event Action<Die> onDieReplenish;

    // Visuals
    public event Action<Entity, bool> onEntityMove;
    public event Action<Entity, bool> onEntityReadyWeapon;
    public event Action<Entity> onEntityMeleeAttack;

    // Stat changes
    public event Action<Entity, int> onEntityTakeDamage;
    public event Action<Entity> onEntityGainExperience;
    public event Action<Entity> onEntityGainLevel;

    public event Action<Entity, int> onPickup;


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

    public void TriggerOnEnterFloor(Dungeon dungeon)
    {
        if (onEnterFloor != null)
        {
            onEnterFloor(dungeon);
        }
    }

    public void TriggerOnExitFloor(Dungeon dungeon)
    {
        if (onExitFloor != null)
        {
            onExitFloor(dungeon);
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

    public void TriggerOnActionSelect(Entity entity, Action action, Dungeon dungeon)
    {
        if (onActionSelect != null)
        {
            onActionSelect(entity, action, dungeon);
        }
    }

    public void TriggerOnLocationSelect(Vector3Int location)
    {
        if (onLocationSelect != null)
        {
            onLocationSelect(location);
        }
    }

    public void TriggerOnActionPerformStart(Entity entity, Action action, Vector3Int location, Dungeon dungeon)
    {
        if (onActionPerformStart != null)
        {
            onActionPerformStart(entity, action, location, dungeon);
        }
    }

    public void TriggerOnActionPerformEnd(Entity entity, Action action, Vector3Int location, Dungeon dungeon)
    {
        if (onActionPerformEnd != null)
        {
            onActionPerformEnd(entity, action, location, dungeon);
        }
    }

    public void TriggerOnEntityMove(Entity entity, bool state)
    {
        if (onEntityMove != null)
        {
            onEntityMove(entity, state);
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

    public void TriggerOnGainExperience(Entity entity)
    {
        if (onEntityGainExperience != null)
        {
            onEntityGainExperience(entity);
        }
    }

    public void TriggerOnGainLevel(Entity entity)
    {
        if (onEntityGainLevel != null)
        {
            onEntityGainLevel(entity);
        }
    }
}
