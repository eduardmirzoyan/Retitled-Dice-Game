using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Normal, Shop, Arena }

public class DataManager : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private Player defaultPlayer;
    [SerializeField] private int maxRooms = 7;
    [SerializeField] private int maxStages = 3;

    [Header("Dynamic Data")]
    [SerializeField] private Player player;
    [SerializeField] private int roomNumber = 1;
    [SerializeField] private int stageNumber = 1;
    [SerializeField] private RoomType currentRoomType;
    [SerializeField] public bool hasEquipmentPrompted;

    public static DataManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (DataManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    public void CreateNewPlayer()
    {
        // Set player to a copy of the template
        player = (Player)defaultPlayer.Copy();

        // Reset progess
        stageNumber = 1;
        roomNumber = 1;
        currentRoomType = RoomType.Normal;
        hasEquipmentPrompted = false;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void SetNextRoom()
    {
        // Increment room number
        roomNumber++;
        if (roomNumber > maxRooms)
        {
            stageNumber++;
            roomNumber = 1;
        }

        // Shops on Room 3 and 6
        if (roomNumber == 3 || roomNumber == 6)
        {
            currentRoomType = RoomType.Shop;
        }
        // Boss on Room 7
        else if (roomNumber == maxRooms)
        {
            currentRoomType = RoomType.Arena;
        }
        else
        {
            currentRoomType = RoomType.Normal;
        }
    }

    public RoomType GetCurrentRoom()
    {
        return currentRoomType;
    }

    public string GetRoomDescription()
    {
        return "Stage " + stageNumber + " - " + roomNumber;
    }

    public RoomType GetNextRoom()
    {
        // Shops on Room 3 and 6
        int next = roomNumber + 1;
        if (next == 3 || next == 6)
        {
            return RoomType.Shop;
        }
        // Boss on Room 7
        else if (next == maxRooms)
        {
            return RoomType.Arena;
        }
        else
        {
            return RoomType.Normal;
        }
    }
}
