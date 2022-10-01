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
    [SerializeField] private int floor = 1;
    [SerializeField] private int gold = 0;


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
        // Initialize
        player.Initialize(defaultPlayer.maxHealth);

        // Reset floor
        floor = 1; 
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void IncrementFloor()
    {
        floor++;
    }

    public int GetFloor()
    {
        return floor;
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
