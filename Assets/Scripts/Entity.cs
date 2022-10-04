using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class Entity : ScriptableObject
{
    [Header("Basic Stats")]
    public new string name;
    public int maxHealth;
    public int currentHealth;
    public int level = 1;
    public int experience = 0;
    public int gold = 0;
    public Inventory inventory;

    [Header("Visuals")]
    public Sprite modelSprite;
    public RuntimeAnimatorController modelController;
    public RuntimeAnimatorController weaponController;
    public GameObject hitEffectPrefab;
    public Vector3 offsetDueToSize;

    [Header("Variable Stats")]
    public AI AI;
    public List<Action> innateActions; // What the entity can do
    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;

    [Header("Dungeon Dependant")]
    public Vector3Int location; // Location of this entity on the floor
    public Room room;

    public void SetRoom(Room dungeon, Vector3Int spawnLocation)
    {
        this.room = dungeon;
        this.location = spawnLocation;
    }

    public void EquipPrimary(Weapon weapon)
    {
        this.primaryWeapon = weapon;

        // Update actions?
    }

    public void EquipSecondary(Weapon weapon)
    {
        this.secondaryWeapon = weapon;

        // Update actions?
    }

    public List<Action> GetActions()
    {
        List<Action> result = new List<Action>();

        // First add innate actions
        result.AddRange(innateActions);

        // Add actions from primay weapon
        result.AddRange(primaryWeapon.actions);

        // Add actions from secondary weapon
        result.AddRange(secondaryWeapon.actions);

        return result;
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
        if (currentHealth == 0)
        {
            // Trigger death
            OnDeath();

            // Remove self from dungeon
            room.Depopulate(this);
        }
    }

    protected virtual void OnDeath()
    {
        // Give this entity's experience to player
        room.player.AddExperience(experience);

        // Give this entity's gold to player
        room.player.AddGold(gold);
    }

    public void Heal(int amount)
    {
        // Debug
        Debug.Log(name + " healed " + amount + " hp.");

        // Heal up to max health
        currentHealth = Mathf.Min(currentHealth + 1, maxHealth);

        // Trigger event
        GameEvents.instance.TriggerOnEntityTakeDamage(this, -amount);
    }

    public void MoveToward(Vector3Int direction)
    {
        // Make sure you are only moving up to 1 tile at a time
        if (direction.magnitude > 1) throw new System.Exception("DIRECTION MAG is NOT 1");

        // Increment location
        location += direction;

        // Interact with new location
        Interact();
    }

    public void WarpTo(Vector3Int location)
    {
        // Set location to warp point
        this.location = location;

        // Interact with new location
        Interact();
    }

    protected virtual void Interact()
    {
        // Does nothing
    }

    public void AttackCurrentLocation()
    {
        var targets = new List<Entity>();

        // Conisder player, enemies and barrels
        targets.Add(room.player);
        targets.AddRange(room.enemies);
        targets.AddRange(room.barrels);

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

    public void AddGold(int amount)
    {
        // If you gain 0, do nothing
        if (amount == 0) return;

        gold += amount;

        // Trigger event
        GameEvents.instance.TriggerOnGainGold(this, amount);
    }

    public void AddExperience(int amount)
    {
        // If you gain 0, do nothing
        if (amount == 0) return;

        // Add amount
        experience += amount;

        // Check if more than threshold, which is 10
        if (experience >= 10)
        {
            // Sub 10
            experience -= 10;

            // Increment level
            level++;

            // Heal to max
            Heal(maxHealth);

            // Trigger event
            GameEvents.instance.TriggerOnGainLevel(this, 1);
        }

        // Trigger event
        GameEvents.instance.TriggerOnGainExperience(this, amount);
    }

    public bool HasNoActionsLeft()
    {
        // Returns true if ALL of your die are exhausted
        return innateActions.All(action => action.die.isExhausted);
    }

    public Entity Copy()
    {
        // Make a copy of itself
        var copy = Instantiate(this);

        // Make a copy of all of it's actions
        for (int i = 0; i < innateActions.Count; i++)
        {
            // Set each action to a copy of itself
            copy.innateActions[i] = innateActions[i].Copy();
        }

        // Make copy of weapons
        if (primaryWeapon != null)
            copy.primaryWeapon = primaryWeapon.Copy();

        if (secondaryWeapon != null)
            copy.secondaryWeapon = secondaryWeapon.Copy();

        // Copy inventory
        if (inventory != null)
            copy.inventory = inventory.Copy();

        return copy;
    }
}
