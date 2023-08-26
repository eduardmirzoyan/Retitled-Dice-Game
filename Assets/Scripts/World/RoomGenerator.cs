using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomGenerator : ScriptableObject
{
    [SerializeField] private int roomWidth = 12;
    [SerializeField] private int roomHeight = 12;
    [SerializeField] private int roomPadding = 8;
    [SerializeField] private int wallSpawnChance = 5;

    [SerializeField] private int shopWidth = 5;
    [SerializeField] private int shopHeight = 5;

    public Room GenerateRoom()
    {
        // Create new room
        var room = ScriptableObject.CreateInstance<Room>();

        // Initialize
        room.Initialize(roomWidth, roomHeight, roomPadding, wallSpawnChance);

        return room;
    }

    public Room GenerateShop()
    {
        // Create new room
        var room = ScriptableObject.CreateInstance<Room>();

        // Initialize with no extra walls
        room.Initialize(shopWidth, shopHeight, roomPadding + (roomWidth - shopWidth), 0);

        return room;
    }
}
