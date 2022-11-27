using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectil : ScriptableObject
{
    [Header("Basic Stats")]
    public int damage;
    public float travelSpeed;

    [Header("Visuals")]
    public Sprite modelSprite;
    public RuntimeAnimatorController modelController;
    public GameObject deathEffectPrefab;

    [Header("Dungeon Dependant")]
    public Vector3Int location; // Location of this entity in the room
    public Room room; // Room this is in
    public Entity entity; // The creator of the projectile

    public void Initialize(Vector3Int location, Entity entity, Room room)
    {
        this.location = location;
        this.entity = entity;
        this.room = room;
    }

    protected virtual void MoveToward(Vector3Int direction)
    {
        // Make sure you are only moving up to 1 tile at a time
        if (direction.magnitude > 1) throw new System.Exception("DIRECTION MAG is NOT 1");

        // Increment location
        this.location += direction;

        // Trigger event
        GameEvents.instance.TriggerOnProjectileMove(this);
    }

    public abstract IEnumerator Travel(Vector3Int direction, int range);
    
    public Projectil Copy()
    {
        // Return a copy
        return Instantiate(this);
    }
}
