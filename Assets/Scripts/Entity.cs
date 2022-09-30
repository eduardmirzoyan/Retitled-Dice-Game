using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class Entity : ScriptableObject
{
    [Header("Basic Stats")]
    public new string name;
    public int maxHealth; // Max Health of this entity
    public int currentHealth; // Current Health of this entity
    public Sprite sprite;
    public GameObject entityModel;


    [Header("Variable Stats")]
    public AI AI;
    public List<Action> actions; // What the entity can do
    public Weapon weapon;


    [Header("Dungeon Dependant")]
    public Vector3Int location; // Location of this entity on the floor
    public Dungeon dungeon;

    public virtual void Initialize(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void SetDungeon(Dungeon dungeon, Vector3Int spawnLocation)
    {
        this.dungeon = dungeon;
        this.location = spawnLocation;
    }

    public void TakeDamage(int amount)
    {
        // Debug
        Debug.Log(name + " took " + amount + " damage.");

        // Reduce health until 0
        currentHealth = Mathf.Max(currentHealth - 1, 0);

        // Trigger event
        GameEvents.instance.TriggerOnEntityTakeDamage(this, 1);

        // Check if dead
        if (currentHealth == 0) {
            // Remove self from dungeon
            dungeon.Depopulate(this);
        }
    }

    public void Heal(int amount)
    {
        // Heal up to max health
        currentHealth = Mathf.Min(currentHealth + 1, maxHealth);
    }

    public void MoveToward(Vector3Int direction)
    {
        // Make sure you are only moving 1 tile at a time
        if (direction.magnitude > 1) throw new System.Exception("DIRECTION MAG is NOT 1");

        // Increment location
        location += direction;

        // This is where interaction happens

        // Pick up any loot/coins

        // Take damage from any enemies?

        // If you are on the exit
        if (dungeon.exitLocation == location)
        {
            // Go to next floor
            GameManager.instance.TravelToNextFloor();
        }
    }

    public void AttackCurrentLocation()
    {
        var targets = new List<Entity>();

        targets.Add(dungeon.player);
        targets.AddRange(dungeon.enemies);

        // Check if any entities are on the same tile, if so damage them
        foreach (var target in targets)
        {
            // If the target is not itself
            if (target.location == location && target != this)
            {
                // Trigger event
                GameEvents.instance.TriggerOnEntityMeleeAttack(this);

                // Currently deal 1 damage, but this might change?
                target.TakeDamage(1);
            }
        }
    }

    public Entity Copy()
    {
        // Make a copy of itself
        var copy = Instantiate(this);

        // Make a copy of all of it's actions
        for (int i = 0; i < actions.Count; i++)
        {
            // If it's not null
            if (actions[i] != null)
            {
                // Set each action to a copy of itself
                copy.actions[i] = actions[i].Copy();
            }
        }

        // Make copy of weapon
        copy.weapon = weapon.Copy();

        return copy;
    }
}
