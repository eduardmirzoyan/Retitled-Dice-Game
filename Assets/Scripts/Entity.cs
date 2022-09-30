using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void TakeDamage(int amount) {
        // Debug
        Debug.Log(name + " took " + amount + " damage.");

        // Reduce health until 0
        currentHealth = Mathf.Max(currentHealth - 1, 0);
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
            GameManager.instance.TravelNextFloor();
        }
    }

    public void AttackCurrentLocation() {
        // Check if any enemies are on the same tile, if so damage them
        foreach (var enemy in dungeon.enemies) {
            // if enemy shares the same tile, damage it
            if (enemy.location == location) {
                // Currently deal 1 damage, but this might change?
                enemy.TakeDamage(1);

                // Trigger event
                GameEvents.instance.TriggerOnEntityTakeDamage(this, enemy, 1);
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

        return copy;
    }
}
