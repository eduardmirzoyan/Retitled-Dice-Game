using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private Player defaultPlayer;

    // This is the template that should be copied when making a new player
    [Header("Dynamic Data")]
    [SerializeField] private Player player;
    [SerializeField] private int roomNumber = 1;
    [SerializeField] private int stageNumber = 1;
    [SerializeField] private int currentRoomIndex = 1;

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
        player = (Player) defaultPlayer.Copy();

        // Create inventory of size 9
        player.inventory = ScriptableObject.CreateInstance<Inventory>();
        player.inventory.Initialize(9);

        // Reset progess
        stageNumber = 1;
        roomNumber = 1;
        currentRoomIndex = -1;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void SetNextRoom(int roomIndex)
    {
        // Check what to do
        switch (roomIndex)
        {
            case 0:
                throw new System.Exception("ROOM INDEX WAS 0");
            case 1:
                // load normal room
                IncrementRoomNumber();
                break;
            case -1:
                // Load shop
                // Don't change floor, but load Shop room
                break;
            default:
                throw new System.Exception("ROOM INDEX WAS UNKNOWN: " + roomIndex);
        }

        // Set room index
        currentRoomIndex = roomIndex;
    }

    private void IncrementRoomNumber()
    {
        // Incrmenet floor
        roomNumber = Mathf.Min(roomNumber + 1, 5);

        // If you reach floor 5, then change the stage
        if (roomNumber == 5)
        {
            stageNumber++;
            roomNumber = 1;
        }
    }

    public int GetRoomIndex() {
        return currentRoomIndex;
    }

    public int GetRoomNumber()
    {
        return roomNumber;
    }

    public string GetRoomDescription() {
        if (currentRoomIndex == 1) {
            return "Stage " + stageNumber + "-" + roomNumber;
        }

        return "Stage " + stageNumber + "-" + "S";
    }

    public int GetNextRoomIndex() {
        // If you are on stage x - 2
        if (roomNumber == 2) {
            // And you are in a combat room now
            if (currentRoomIndex == 1) {
                // Next room should be a shop
                return -1;
            }
        }

        return 1;
    }

    public void Save()
    {
        // TODO
    }

    public void Load()
    {
        // TODO
    }
}
