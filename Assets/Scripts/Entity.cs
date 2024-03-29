using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Entity : ScriptableObject
{
    [Header("Basic Stats")]
    public new string name;
    public int maxHealth;
    public int currentHealth;
    public int gold;
    public Inventory inventory;

    [Header("Visuals")]
    public Sprite modelSprite;
    public RuntimeAnimatorController modelController;

    [Header("Variable Stats")]
    public AI AI;
    public List<Action> innateActions;
    public List<Weapon> weapons = new List<Weapon>(2);
    public List<EntityEnchantment> enchantments;

    [Header("Dynamic Data")]
    public Vector3Int location; // Location of this entity on the floor
    public Room room;
    public EntityModel model;

    public void Initialize(Room room, Vector3Int location)
    {
        this.room = room;
        this.location = location;

        // Initialize actions
        foreach (var actions in innateActions)
        {
            actions.Initialize(null);
        }

        // Initialize weapons
        foreach (var weapon in weapons)
        {
            if (weapon != null)
                weapon.Initialize(this);
        }

        // Initialize enchantments
        foreach (var enchantment in enchantments)
        {
            enchantment.Initialize(this);
        }
    }

    public void Uninitialize()
    {
        // Unintialize actions
        foreach (var actions in innateActions)
        {
            actions.Uninitialize();
        }

        // Unintialize weapons
        foreach (var weapon in weapons)
        {
            if (weapon != null)
                weapon.Uninitialize();
        }

        // Unintialize all enchantments
        foreach (var enchantment in enchantments)
        {
            enchantment.Uninitialize();
        }
    }

    public void EquipWeapon(Weapon weapon, int index)
    {
        // Set new weapon
        var oldWeapon = this.weapons[index];
        this.weapons[index] = weapon;

        // Check if we had a weapon before here
        if (oldWeapon != null)
        {
            // Disable enchantments
            oldWeapon.Uninitialize();

            // Trigger event
            GameEvents.instance.TriggerOnUnequipWeapon(this, oldWeapon, index);
        }

        // Check if we have a weapon now
        if (this.weapons[index] != null)
        {
            // Enable enchantments
            this.weapons[index].Initialize(this);

            // Trigger event
            GameEvents.instance.TriggerOnEquipWeapon(this, this.weapons[index], index);
        }
    }

    public void GainEnchantment(EntityEnchantment entityEnchantment)
    {
        // Initialize it
        entityEnchantment.Initialize(this);

        // Keep track
        enchantments.Add(entityEnchantment);

        // Trigger event
        GameEvents.instance.TriggerOnEntityGainEnchantment(this, entityEnchantment);
    }

    public List<Action> AllActions()
    {
        List<Action> result = new List<Action>();

        // First add innate actions
        if (innateActions != null)
            result.AddRange(innateActions);

        // Add actions from primay weapon
        if (weapons[0] != null)
            result.AddRange(weapons[0].actions);

        // Add actions from secondary weapon
        if (weapons[1] != null)
            result.AddRange(weapons[1].actions);

        return result;
    }

    public void TakeDamageFrom(Entity source, Weapon weapon, int amount)
    {
        // Debug
        // Debug.Log(name + " took " + amount + " damage.");

        // Reduce health until 0
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        model.TakeDamage(amount);

        // Trigger event
        GameEvents.instance.TriggerOnEntityTakeDamage(this, amount);

        // Check if dead
        if (currentHealth == 0)
        {
            // Cancel any actions
            GameManager.instance.ClearDelayedActions(this);

            // Give gold to killer (usually player)
            source.AddGold(gold);

            // Trigger event
            GameEvents.instance.TriggerOnEntityKillEntity(source, weapon, this);

            // Remove self from dungeon
            room.DespawnEntity(this);
        }
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

    public void Relocate(Vector3Int newLocation)
    {
        var newTile = room.TileFromLocation(newLocation);
        var oldTile = room.TileFromLocation(location);

        // Error check
        if (newTile.containedEntity != null)
            throw new System.Exception($"Tile at {newLocation} is already occupied by {newTile.containedEntity.name}");

        // Update tiles
        oldTile.containedEntity = null;
        newTile.containedEntity = this;

        // Update this entity location
        location = newTile.location;

        // Interact with new location
        Interact();

        // Trigger event
        GameEvents.instance.TriggerOnEntityRelocate(this);
    }

    protected virtual void Interact()
    {
        // Does nothing by default
    }

    public void AttackEntity(Entity target, Weapon weapon, int amount)
    {
        target.TakeDamageFrom(this, weapon, amount);
    }

    public void AttackLocation(Vector3Int location, Weapon weapon, int amount)
    {
        var tile = room.TileFromLocation(location);
        if (tile.containedEntity != null)
        {
            tile.containedEntity.TakeDamageFrom(this, weapon, amount);
        }
    }

    public void AddGold(int amount)
    {
        // If you gain 0, do nothing
        if (amount == 0) return;

        gold += amount;

        // Trigger event
        GameEvents.instance.TriggerOnGoldChange(this, amount);
    }

    public Entity Copy()
    {
        if (weapons == null || weapons.Count == 0)
            throw new System.Exception($"Entity {name} has not had their weapons intialized.");

        // Make a copy of itself
        var copy = Instantiate(this);

        // Make a copy of all of it's actions
        for (int i = 0; i < innateActions.Count; i++)
        {
            // Set each action to a copy of itself
            copy.innateActions[i] = innateActions[i].Copy();
        }

        // Make a copy of all of it's actions
        for (int i = 0; i < weapons.Count; i++)
        {
            // Set each action to a copy of itself
            if (weapons[i] != null)
                copy.weapons[i] = (Weapon)weapons[i].Copy();
        }

        // Make a copy of all of it's enchantments
        for (int i = 0; i < enchantments.Count; i++)
        {
            copy.enchantments[i] = enchantments[i].Copy();
            copy.enchantments[i].entity = copy;
        }

        // Copy inventory
        if (inventory != null)
            copy.inventory = inventory.Copy();

        return copy;
    }
}
