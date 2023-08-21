using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomGenerator : ScriptableObject
{
    [SerializeField] private int roomWidth = 12;
    [SerializeField] private int roomHeight = 12;
    [SerializeField] private int roomPadding = 8;
    [SerializeField] private int goldSpawnChance = 3;
    [SerializeField] private int barrelSpawnChance = 3;
    [SerializeField] private Vector2Int wallSpawnChanceDistribution = new Vector2Int(5, 10);
    [SerializeField] private Barrel barrel;

    public Room GenerateRoom()
    {
        // Create new room
        var room = ScriptableObject.CreateInstance<Room>();

        // Number of keys equals to the floor number
        var numKeys = DataManager.instance.GetRoomNumber();

        // Randomly choose a percentage
        int wallSpawnChance = Random.Range(wallSpawnChanceDistribution.x, wallSpawnChanceDistribution.y + 1);

        // Initialize
        room.Initialize(roomWidth, roomHeight, roomPadding, numKeys, goldSpawnChance, wallSpawnChance, barrelSpawnChance, barrel);

        return room;
    }

    public Room GenerateShop()
    {
        // Create new room
        var room = ScriptableObject.CreateInstance<Room>();

        // Initialize with no extra walls, barrels, gold or keys
        room.Initialize(roomWidth, roomHeight, roomPadding, 0, 0, 0, 0, barrel);

        return room;
    }
}
