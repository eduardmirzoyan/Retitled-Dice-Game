using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType { None, Gold, Key }
public enum TileType { Chasam, Floor, Wall, Entrance, Exit }

[System.Serializable]
public class RoomTile : ScriptableObject
{
    [Header("Properties")]
    public Vector3Int location;
    public TileType tileType;
    public PickUpType containedPickup;
    public Entity containedEntity;

    [Header("Parent References")]
    public Room room;

    public void Initialize(Vector3Int location, TileType tileType, Room room)
    {
        this.location = location;
        this.tileType = tileType;
        this.room = room;

        // Set contained to empty
        containedPickup = PickUpType.None;
        containedEntity = null;

        // Set name of SO based on coordinate
        name = "Tile " + location.ToString();
    }
}
