using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType { None, Coin, Key }
public enum TileType { Void, Floor, Wall, Entrance, Exit }

[System.Serializable]
public class RoomTile : ScriptableObject
{
    [Header("Properties")]
    public Vector3Int location;
    public TileType tileType;
    public PickUpType containedPickup;
    public Entity containedEntity;
    public int preemptiveThreats;
    public int reactiveThreats;

    [Header("Parent References")]
    public Room room;

    public void Initialize(Vector3Int location, TileType tileType, PickUpType containedPickup, Room room)
    {
        this.location = location;
        this.tileType = tileType;
        this.containedPickup = containedPickup;
        this.room = room;

        // Set name of SO based on coordinate
        name = "Tile " + location.ToString();
    }

    public void SetEntity(Entity entity)
    {
        this.containedEntity = entity;

        // Trigger event?
        GameEvents.instance.TriggerOnEntityEnterTile(entity, location);
    }

    public void Threaten(bool isReactive)
    {
        if (isReactive)
            reactiveThreats++;
        else
            preemptiveThreats++;

        // Logic of highlighting here
        GameEvents.instance.TriggerOnEntityWatchLocation(null, location);
    }

    public void Dethreaten(bool isReactive)
    {
        if (isReactive)
            reactiveThreats = Mathf.Max(reactiveThreats - 1, 0);
        else
            preemptiveThreats = Mathf.Max(preemptiveThreats - 1, 0);

        // Logic of highlighting here
    }
}
