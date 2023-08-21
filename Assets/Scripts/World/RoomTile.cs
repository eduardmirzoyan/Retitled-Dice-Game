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
}
