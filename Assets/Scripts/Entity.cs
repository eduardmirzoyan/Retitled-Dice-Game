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
    public int level = 1;
    public int experience = 0;
    public int gold = 0;
    public Inventory inventory;

    [Header("Visuals")]
    public Sprite modelSprite;
    public RuntimeAnimatorController modelController;
    public GameObject hitEffectPrefab;

    [Header("Variable Stats")]
    public AI AI;
    public List<Action> innateActions;
    public List<Weapon> weapons = new List<Weapon>(2);

    [Header("Dynamic Data")]
    public Vector3Int location; // Location of this entity on the floor
    public Room room;

    public void Initialize(Room dungeon, Vector3Int spawnLocation)
    {
        this.room = dungeon;
        this.location = spawnLocation;
    }

    public void EquipWeapon(Weapon weapon, int index)
    {
        // WORK AROUND

        // Set new weapon
        var oldWeapon = this.weapons[index];
        this.weapons[index] = weapon;

        // Check if we had a weapon before here
        if (oldWeapon != null)
        {
            // Trigger event
            GameEvents.instance.TriggerOnUnequipWeapon(this, this.weapons[index], index);
        }

        // Check if we have a weapon now
        if (this.weapons[index] != null)
        {
            // Trigger event
            GameEvents.instance.TriggerOnEquipWeapon(this, this.weapons[index], index);
        }
    }

    public List<Action> GetActions()
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
            // Cancel any actions
            GameManager.instance.ClearDelayedActions(this);
            GameManager.instance.ClearReativeActions(this);

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

    public IEnumerator MoveToward(Vector3Int direction)
    {
        // Move within room
        room.MoveEntityToward(this, direction);

        // Trigger event
        GameEvents.instance.TriggerOnEntityMove(this);

        // Wait for animation
        yield return new WaitForSeconds(GameManager.instance.gameSettings.moveBufferTime);

        // Interact with new location
        Interact();

        // Check for any reactive actions
        if (this is Player)
            yield return GameManager.instance.PerformReactiveAction(location);
    }

    public IEnumerator WarpTo(Vector3Int location)
    {
        // Move
        room.MoveEntityTo(this, location);

        // Trigger event
        GameEvents.instance.TriggerOnEnityWarp(this);

        // Wait for animation
        yield return new WaitForSeconds(GameManager.instance.gameSettings.warpBufferTime);

        // Interact with new location
        Interact();

        // Check for any reactive actions
        if (this is Player)
            yield return GameManager.instance.PerformReactiveAction(location);
    }

    public IEnumerator Jump(Vector3Int location)
    {

        // Move
        room.MoveEntityTo(this, location);

        // Trigger event
        GameEvents.instance.TriggerOnEntityJump(this);

        // Wait for animation
        yield return new WaitForSeconds(GameManager.instance.gameSettings.jumpBufferTime);

        // Interact with new location
        Interact();

        // Check for any reactive actions
        if (this is Player)
            yield return GameManager.instance.PerformReactiveAction(location);


    }

    protected virtual void Interact()
    {
        // Does nothing for now
    }

    public void Attack(Entity target)
    {
        // Currently deal 1 damage, but this might change?
        target.TakeDamage(1);
    }

    public void AddGold(int amount)
    {
        // If you gain 0, do nothing
        if (amount == 0) return;

        gold += amount;

        // Trigger event
        GameEvents.instance.TriggerOnGoldChange(this, amount);
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
        if (weapons == null || weapons.Count == 0)
            throw new System.Exception("Entity " + name + " has not had their weapons intialized.");

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

        // Copy inventory
        if (inventory != null)
            copy.inventory = inventory.Copy();

        return copy;
    }
}
