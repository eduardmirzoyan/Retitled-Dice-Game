using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Dungeon : ScriptableObject
{
    public int width;
    public int height;
    public int padding;

    public List<List<int>> floor; // 0 = empty, 1 = exists, null is problem
    public List<List<int>> walls; // 0 = empty, 1 = exists, null is problem

    public Vector3Int entranceLocation;
    public Vector3Int exitLocation;

    public List<Entity> enemies; // TODO

    public Player player;

    public void Initialize(int width, int height, int padding, int numberOfEnemies, Player player)
    {
        // Save dimensions
        this.width = width;
        this.height = height;
        this.padding = padding;

        // Save player
        this.player = player;

        // Generate floor
        GenerateFloor();

        // Generate walls
        GenerateWalls();

        // Generate entrance
        GenerateEntrance();

        // Generate exit
        GenerateExit();

        // Set the player's location to be at dungeon entrance
        player.SetDungeon(this, entranceLocation);

        // Set enemies to this dungeon
        // TODO
    }

    private void GenerateFloor()
    {
        // Initilize floor
        floor = new List<List<int>>();

        // Fill floor with values
        for (int i = 0; i < width + 2 * padding; i++)
        {
            // Make new row
            floor.Add(new List<int>());
            for (int j = 0; j < height + 2 * padding; j++)
            {
                // Add 1 to indicate floor
                floor[i].Add(1);
            }
        }
    }

    private void GenerateWalls()
    {
        // Initilize walls based on padding
        walls = new List<List<int>>();

        // Fill floor with values
        for (int i = 0; i < width + 2 * padding; i++)
        {
            // Make new row
            walls.Add(new List<int>());
            for (int j = 0; j < height + 2 * padding; j++)
            {
                // If you are in the padding range then add wall
                if (i < padding || i >= width + padding || j < padding || j >= height + padding)
                {
                    // Create a wall
                    walls[i].Add(1);
                }
                else
                {
                    // No wall
                    walls[i].Add(0);
                }
            }
        }
    }

    private void GenerateEntrance()
    {
        // Current algorithm: Randomly choose a corner in the room

        // Get all 4 corners
        Vector3Int[] corners = {new Vector3Int(padding + 1, padding + 1), new Vector3Int(padding + 1, padding + height - 2),
                                new Vector3Int(padding + width - 2, padding + height - 2), new Vector3Int(padding + width - 2, padding + 1)};

        // Set entrance
        entranceLocation = corners[Random.Range(0, corners.Length)];
    }

    private void GenerateExit()
    {
        // Current algorithm: Choose the corner opposite of the entrance

        exitLocation = new Vector3Int(2 * padding + width - entranceLocation.x - 1, 2 * padding + height - entranceLocation.y - 1);
    }

    private void GenerateEnemies(int numberOfEnemies)
    {
        // Initialize enemies
        enemies = new List<Entity>(numberOfEnemies);

        // TODO
    }
}
