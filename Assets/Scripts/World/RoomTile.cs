using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType { None, Gold, Key }
public enum TileType { Void, Floor, Wall, Entrance, Exit }

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

    public void Initialize(Vector3Int location, Room room)
    {
        this.location = location;
        this.tileType = TileType.Floor;
        this.containedPickup = PickUpType.None;
        this.room = room;
        containedEntity = null;

        // Set name of SO based on coordinate
        name = "Tile " + location.ToString();
    }
}
