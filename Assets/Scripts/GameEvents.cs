using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public event Action<Dungeon> onEnterFloor;
    public event Action<Entity> onGenerateEntity;

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
    public event Action<Entity, Vector3Int, Vector3Int> onEntityMove;
    public event Action<Entity, Entity, int> onEntityTakeDamage; // TODO


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
        DontDestroyOnLoad(this);
    }

    public void TriggerOnEnterFloor(Dungeon dungeon)
    {
        if (onEnterFloor != null)
        {
            onEnterFloor(dungeon);
        }
    }

    public void TriggerOnGenerateEnity(Entity entity)
    {
        if (onGenerateEntity != null)
        {
            onGenerateEntity(entity);
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

    public void TriggerOnEntityMove(Entity entity, Vector3Int from, Vector3Int to)
    {
        if (onEntityMove != null)
        {
            onEntityMove(entity, from, to);
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

    public void TriggerOnEntityTakeDamage(Entity attacker, Entity target, int damage)
    {
        if (onEntityTakeDamage != null)
        {
            onEntityTakeDamage(attacker, target, damage);
        }
    }
}
