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

[CreateAssetMenu(menuName = "Generator/Room")]
public class RoomGenerator : ScriptableObject
{
    [Header("Regular Data")]
    [SerializeField] private Vector2Int roomSize = new Vector2Int(10, 10);
    [SerializeField][Range(0f, 1f)] private float floorChance;

    [Header("Shop Data")]
    [SerializeField] private Vector2Int shopSize = new Vector2Int(5, 5);

    [Header("Shop Data")]
    [SerializeField] private Vector2Int arenaSize = new Vector2Int(10, 10);
    [SerializeField][Range(0f, 1f)] private float arenaFloorChance;

    public Room GenerateCustomRoom()
    {
        // Create new room
        Room room = CreateInstance<Room>();

        // Initialize
        room.Initialize(roomSize.x, roomSize.y, floorChance);

        return room;
    }

    public Room GenerateShop()
    {
        // Create new room
        Room room = CreateInstance<Room>();

        // Initialize with no extra walls or chasams
        room.Initialize(shopSize.x, shopSize.y, 1f);

        return room;
    }

    public Room GenerateArena()
    {
        // Create new room
        Room room = CreateInstance<Room>();

        // Initialize with no extra walls or chasams
        room.Initialize(arenaSize.x, arenaSize.y, arenaFloorChance);

        return room;
    }

    public Room GenerateRoom(RoomSize roomSize)
    {
        // Create new room
        Room room = CreateInstance<Room>();

        switch (roomSize)
        {
            case RoomSize.Tiny:
                // Initialize
                room.Initialize(4, 4, floorChance);
                break;
            case RoomSize.Small:
                // Initialize
                room.Initialize(5, 5, floorChance);
                break;
            case RoomSize.Medium:
                // Initialize
                room.Initialize(6, 6, floorChance);
                break;
            case RoomSize.Large:
                // Initialize
                room.Initialize(7, 7, floorChance);
                break;
            case RoomSize.Massive:
                // Initialize
                room.Initialize(8, 8, floorChance);
                break;
        }

        return room;
    }
}
