using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Entity : ScriptableObject
{
    public new string name;

    public int maxHealth; // Max Health of this entity
    public int currentHealth; // Current Health of this entity
    public Vector3Int location; // Location of this entity on the board

    public List<Action> actions; // What the entity can do

    public Sprite sprite;
    public GameObject entityModel;
    public Dungeon dungeon;

    public virtual void Initialize(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void SetDungeon(Dungeon dungeon, Vector3Int spawnLocation) {
        this.dungeon = dungeon;
        this.location = spawnLocation;
    }

    public void MoveToward(Vector3Int direction) {
        // Make sure you are only moving 1 tile at a time
        if (direction.magnitude > 1) throw new System.Exception("DIRECTION MAG is NOT 1");

        // Increment location
        location += direction;

        // This is where interaction happens
        
        // Pick up any loot/coins

        // Take damage from any enemies

        // If you are on the exit
        if (dungeon.exitLocation == location) {
            // Go to next floor
            GameManager.instance.TravelNextFloor();
        }
    }

    public Entity Copy() {
        // Make a copy of itself
        var copy = Instantiate(this);

        // Make a copy of all of it's actions
        for (int i = 0; i < actions.Count; i++)
        {
            // If it's not null
            if (actions[i] != null) {
                // Set each action to a copy of itself
                copy.actions[i] = actions[i].Copy();
            }
        }

        return copy;
    }
}
