using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.MPE;
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
    public List<Action> innateActions; // What the entity can do
    public Weapon mainWeapon;
    public Weapon offWeapon;

    [Header("Dynamic Data")]
    public Vector3Int location; // Location of this entity on the floor
    public Room room;

    public void Initialize(Room dungeon, Vector3Int spawnLocation)
    {
        this.room = dungeon;
        this.location = spawnLocation;
    }

    public void EquipMainhand(Weapon weapon)
    {
        var curWeapon = mainWeapon;
        this.mainWeapon = weapon;

        // If already have equipment
        if (curWeapon != null)
        {
            // Trigger event
            GameEvents.instance.TriggerOnUnequipMainhand(this, curWeapon);
        }

        if (weapon != null)
        {
            // Trigger event
            GameEvents.instance.TriggerOnEquipMainhand(this, weapon);
        }
    }

    public void EquipOffhand(Weapon weapon)
    {
        var curWeapon = offWeapon;
        this.offWeapon = weapon;

        // If already have equipment
        if (curWeapon != null)
        {
            // Trigger event
            GameEvents.instance.TriggerOnUnequipOffhand(this, curWeapon);
        }

        if (weapon != null)
        {
            // Trigger event
            GameEvents.instance.TriggerOnEquipOffhand(this, weapon);
        }
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

        // Interact with new location
        Interact();

        // Check for any reactive actions
        if (this is Player)
            yield return GameManager.instance.PerformReactiveAction(location);

        // Wait for animation
        yield return new WaitForSeconds(GameManager.instance.gameSettings.warpBufferTime);
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
