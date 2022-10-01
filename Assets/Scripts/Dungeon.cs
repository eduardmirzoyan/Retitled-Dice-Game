using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct FloorTile {
    public int gold;
    public bool hasKey;

    public FloorTile(int gold, bool hasKey) {
        this.gold = gold;
        this.hasKey = hasKey;
    }
}

[CreateAssetMenu]
public class Dungeon : ScriptableObject
{
    public int width;
    public int height;
    public int padding;

    /// <summary>
    /// 0 = empty; 1 = exists
    /// </summary>
    public List<List<int>> floor;

    /// <summary>
    /// 0 = empty; 1 = exists
    /// </summary>
    public List<List<int>> walls;

    /// <summary>
    /// 0 = empty; 1 = key; 2 = gold
    /// </summary>
    public List<List<int>> pickups;

    public Vector3Int entranceLocation;
    public Vector3Int exitLocation;

    public List<Entity> enemies;
    public Player player;

    public void Initialize(int width, int height, int padding)
    {
        // Save dimensions
        this.width = width;
        this.height = height;
        this.padding = padding;

        // Generate floor
        GenerateFloor();

        // Generate walls
        GenerateWalls();

        // Generate entrance
        GenerateEntrance();

        // Generate exit
        GenerateExit();

        // Initalize lists
        enemies = new List<Entity>();
    }

    public void Populate(Entity entity)
    {
        if (entity is Player)
        {
            this.player = (Player)entity;
            // Set the player's location to be at dungeon entrance
            player.SetDungeon(this, entranceLocation);
        }
        else
        {
            // Set spawn location
            Vector3Int spawnLocation = new Vector3Int(padding + width / 2, padding + height / 2 + enemies.Count, 0);

            // Set dungeon to this
            entity.SetDungeon(this, spawnLocation);

            // Save
            enemies.Add(entity);
        }

        // Trigger event
        GameEvents.instance.TriggerOnSpawnEnity(entity);
    }

    public void Depopulate(Entity entity)
    {
        // Check if entity is the player
        if (entity == player)
        {
            // Debug
            Debug.Log("GAME OVER!");

            // GAME OVER!
        }
        // Else attempt to remove it from enemies
        else if (enemies.Remove(entity))
        {
            // Enemy was removed
        }
        else
        {
            // Debug
            Debug.Log(entity.name + " was attempted to be removed but didn't exist in dungeon.");
        }

        // Trigger event
        GameEvents.instance.TriggerOnRemoveEnity(entity);
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

    public void GeneratePickups() {
        // Initialize
        pickups = new List<List<int>>();
        
        // Loop through all tiles
        for (int i = 0; i < floor.Count; i++)
        {
            // Make new row
            pickups.Add(new List<int>());
            for (int j = 0; j < floor[i].Count; j++)
            {
                // Get vector
                Vector3Int pos = new Vector3Int(i, j);
                int value = 0;

                // If tile is floor AND is not a wall, enemy, player or entrance/exit
                if (floor[i][j] == 1 && walls[i][j] == 0 && enemies.All(enemy => enemy.location != pos) && player.location != pos && entranceLocation != pos && exitLocation != pos) {
                    // Generate gold on tile by chance, 5%
                    value = Random.Range(0, 100) <= 5 ? 2 : 0;
                }

                // Add tile
                pickups[i].Add(value);
            }
        }
    }

    public bool IsValidLocation(Vector3Int location, bool ignoreEntity = false)
    {
        // Make sure there is no wall
        if (walls[location.x][location.y] != 0)
        {
            return false;
        }

        // Make sure there is no player or ANY enemy there
        if (!ignoreEntity)
        {
            if (player.location == location || enemies.Any((enemy) => (enemy.location == location)))
            {
                return false;
            }
        }

        // Else you're good to go :)
        return true;
    }

    public bool IsValidPath(Vector3Int start, Vector3Int end, bool ignoreEntity = false) {
        // Get direction
        Vector3Int direction = end - start;
        if (direction.x > 0) // Move right
        {
            direction.x = 1;
        }
        else if (direction.x < 0) // Move left
        {
            direction.x = -1;
        }
        else if (direction.y > 0) // Move up
        {
            direction.y = 1;
        }
        else if (direction.y < 0) // Move down
        {
            direction.y = -1;
        }
        else {  }

        // Keep looping until start is at the end
        while (start != end) {

            // Check to see if the location is valid
            if (!IsValidLocation(start + direction, ignoreEntity)) {
                return false;
            }

            // Increment start
            start += direction;
        }

        // // Make sure there is no enemy on the last tile regardless of conditions
        if (!IsValidLocation(end)) {
            return false;
        }

        // Else we good :)
        return true;
    }
}
