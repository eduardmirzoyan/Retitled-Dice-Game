using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

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
    public GameObject hitEffectPrefab;
    public Vector3 offsetDueToSize;

    [Header("Variable Stats")]
    public AI AI;
    public List<Action> innateActions; // What the entity can do
    public Weapon mainWeapon;
    public Weapon offWeapon;

    [Header("Dungeon Dependant")]
    public Vector3Int location; // Location of this entity on the floor
    public Room room;
    public (Action, Vector3Int) preparedAction;

    public void Initialize(Room dungeon, Vector3Int spawnLocation)
    {
        this.room = dungeon;
        this.location = spawnLocation;
    }

    public void EquipPrimary(Weapon weapon)
    {
        this.mainWeapon = weapon;

        // Trigger event
        GameEvents.instance.TriggerOnWeaponEquip(this, weapon);
    }

    public void EquipSecondary(Weapon weapon)
    {
        this.offWeapon = weapon;

        // Trigger event
        GameEvents.instance.TriggerOnWeaponEquip(this, weapon);
    }

    public List<Action> GetActions()
    {
        List<Action> result = new List<Action>();

        // First add innate actions
        if (innateActions != null)
            result.AddRange(innateActions);

        // Add actions from primay weapon
        if (mainWeapon != null)
            result.AddRange(mainWeapon.actions);

        // Add actions from secondary weapon
        if (offWeapon != null)
            result.AddRange(offWeapon.actions);

        return result;
    }

    public void TakeDamage(int amount)
    {
        // Debug
        Debug.Log(name + " took " + amount + " damage.");

        // Reduce health until 0
        currentHealth = Mathf.Max(currentHealth - 1, 0);

        // Trigger event
        GameEvents.instance.TriggerOnEntityTakeDamage(this, amount);

        // Check if dead
        if (currentHealth == 0)
        {
            // Trigger death
            OnDeath();

            // Remove self from dungeon
            room.DespawnEntity(this);
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
        // Move within room
        room.MoveEntityToward(this, direction);

        // Trigger event
        GameEvents.instance.TriggerOnEntityMove(this);

        // Interact with new location
        Interact();
    }

    public void WarpTo(Vector3Int location)
    {
        // Move
        room.MoveEntityTo(this, location);

        // Interact with new location
        Interact();
    }

    protected virtual void Interact()
    {
        // Does nothing for now
    }

    public bool MeleeAttackLocation(Vector3Int location, Weapon weapon = null)
    {
        var target = room.GetEntityAtLocation(location);
        if (target != null)
        {
            // Attack target
            MeleeAttackEntity(target, weapon);

            return true;
        }

        return false;
    }

    public void MeleeAttackEntity(Entity target, Weapon weapon = null)
    {
        // Currently deal 1 damage, but this might change?
        target.TakeDamage(1);

        // Trigger event
        if (weapon != null)
            GameEvents.instance.TriggerOnEntityMeleeAttack(this, weapon);
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

    public bool IsFullHealth()
    {
        return currentHealth >= maxHealth;
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
        if (mainWeapon != null)
            copy.mainWeapon = (Weapon)mainWeapon.Copy();

        if (offWeapon != null)
            copy.offWeapon = (Weapon)offWeapon.Copy();

        // Copy inventory
        if (inventory != null)
            copy.inventory = inventory.Copy();

        return copy;
    }
}
