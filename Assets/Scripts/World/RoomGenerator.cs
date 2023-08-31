using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Room GenerateRoom()
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
        room.Initialize(shopSize.x, shopSize.y, roomPadding + (roomSize.x - shopSize.x), 0, 0);

        return room;
    }

    public Room GenerateArena()
    {
        // Create new room
        var room = ScriptableObject.CreateInstance<Room>();

        // Initialize with no extra walls or chasams
        room.Initialize(arenaSize.x, arenaSize.y, roomPadding + (roomSize.x - arenaSize.x), arenaWallSpawnChance, arenaChasamSpawnChance);

        return room;
    }
}
