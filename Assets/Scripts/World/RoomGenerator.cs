using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomSize
{
    Tiny, // 4x4
    Small, // 6x6
    Medium, // 8x8
    Large, // 10x10
    Massive // 12x12
}

[CreateAssetMenu]
public class RoomGenerator : ScriptableObject
{
    [Header("Regular Data")]
    [SerializeField] private Vector2Int roomSize = new Vector2Int(10, 10);
    [SerializeField] private int roomPadding = 8;
    [SerializeField] private int wallSpawnChance = 5;
    [SerializeField] private int chasamSpawnChance = 5;

    [Header("Shop Data")]
    [SerializeField] private Vector2Int shopSize = new Vector2Int(5, 5);

    [Header("Shop Data")]
    [SerializeField] private Vector2Int arenaSize = new Vector2Int(10, 10);
    [SerializeField] private int arenaWallSpawnChance = 0;
    [SerializeField] private int arenaChasamSpawnChance = 5;

    public Room GenerateCustomRoom()
    {
        // Create new room
        var room = ScriptableObject.CreateInstance<Room>();

        // Initialize
        room.Initialize(roomSize.x, roomSize.y, roomPadding, wallSpawnChance, chasamSpawnChance);

        return room;
    }

    public Room GenerateShop()
    {
        // Create new room
        var room = ScriptableObject.CreateInstance<Room>();

        // Initialize with no extra walls or chasams
        room.Initialize(shopSize.x, shopSize.y, roomPadding, 0, 0);

        return room;
    }

    public Room GenerateArena()
    {
        // Create new room
        var room = ScriptableObject.CreateInstance<Room>();

        // Initialize with no extra walls or chasams
        room.Initialize(arenaSize.x, arenaSize.y, roomPadding, arenaWallSpawnChance, arenaChasamSpawnChance);

        return room;
    }

    public Room GenerateRoom(RoomSize roomSize)
    {
        // Create new room
        var room = ScriptableObject.CreateInstance<Room>();

        switch (roomSize)
        {
            case RoomSize.Tiny:
                // Initialize
                room.Initialize(4, 4, roomPadding, wallSpawnChance, chasamSpawnChance);
                break;
            case RoomSize.Small:
                // Initialize
                room.Initialize(6, 6, roomPadding, wallSpawnChance, chasamSpawnChance);
                break;
            case RoomSize.Medium:
                // Initialize
                room.Initialize(8, 8, roomPadding, wallSpawnChance, chasamSpawnChance);
                break;
            case RoomSize.Large:
                // Initialize
                room.Initialize(10, 10, roomPadding, wallSpawnChance, chasamSpawnChance);
                break;
            case RoomSize.Massive:
                // Initialize
                room.Initialize(12, 12, roomPadding, wallSpawnChance, chasamSpawnChance);
                break;
        }

        return room;
    }
}
