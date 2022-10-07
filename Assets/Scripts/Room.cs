using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class Room : ScriptableObject
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

    public int numKeys;

    public Vector3Int entranceLocation;
    public RoomExit roomExit;

    public List<Entity> barrels;
    public List<Entity> enemies;
    public Player player;

    private Barrel barrel;

    private int goldSpawnChance;
    private int wallSpawnChance;
    private int barrelSpawnChance;

    public void Initialize(int width, int height, int padding, int numKeys, int goldSpawnChance, int wallSpawnChance, int barrelSpawnChance, Barrel barrel)
    {
        // Save dimensions
        this.width = width;
        this.height = height;
        this.padding = padding;
        this.numKeys = numKeys;
        this.goldSpawnChance = goldSpawnChance;
        this.wallSpawnChance = wallSpawnChance;
        this.barrelSpawnChance = barrelSpawnChance;
        this.barrel = barrel;

        // Generate floor
        GenerateFloor();

        // Generate walls
        GenerateWalls();

        // Generate entrance
        GenerateEntrance();

        // Generate exit
        GenerateExit();

        // Generate barrels
        GenerateBarrels();

        // Generate pickups
        GeneratePickups();

        // Initalize lists
        enemies = new List<Entity>();
    }

    public void Populate(Entity entity)
    {
        if (entity is Player)
        {
            this.player = (Player)entity;
            // Set the player's location to be at dungeon entrance
            player.SetRoom(this, entranceLocation);
        }
        else
        {
            // Save location
            Vector3Int spawnLocation;

            do // Get random free point in dungeon
            {

                int randX = Random.Range(padding, width + padding);
                int randY = Random.Range(padding, height + padding);
                spawnLocation = new Vector3Int(randX, randY);

            } while (!IsValidLocation(spawnLocation));

            // Set dungeon to this
            entity.SetRoom(this, spawnLocation);

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
        else if (barrels.Remove(entity))
        {
            // Barrel was removed
        }
        else
        {
            // Debug
            Debug.Log(entity.name + " was attempted to be removed but didn't exist in dungeon.");
        }

        // Trigger event
        GameEvents.instance.TriggerOnRemoveEnity(entity);
    }

    public Entity[] GetAllEntities()
    {
        return new Entity[] { player }.Concat(enemies).ToArray();
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
                    // Randomly assign wall, 5% or spawnchance
                    int value = value = Random.Range(0, 100) < wallSpawnChance ? 1 : 0;
                    // No wall
                    walls[i].Add(value);
                }
            }
        }
    }

    private void GenerateEntrance()
    {
        // Current algorithm: Randomly choose a corner in the room ?

        // Get all 4 corners
        Vector3Int[] corners = {new Vector3Int(padding + 1, padding + 1), new Vector3Int(padding + 1, padding + height - 2),
                                new Vector3Int(padding + width - 2, padding + height - 2), new Vector3Int(padding + width - 2, padding + 1)};

        // Set entrance
        entranceLocation = corners[Random.Range(0, corners.Length)];

        // Set any wall there to 0
        walls[entranceLocation.x][entranceLocation.y] = 0;
    }

    private void GenerateExit()
    {
        // Create SO
        roomExit = ScriptableObject.CreateInstance<RoomExit>();

        // Current algorithm: Choose the corner opposite of the entrance
        var exitLocation = new Vector3Int(2 * padding + width - entranceLocation.x - 1, 2 * padding + height - entranceLocation.y - 1);

        // Get next room index
        var nextRoomIndex = DataManager.instance.GetNextRoomIndex();

        // Initalize exit
        roomExit.Initialize(exitLocation, numKeys, nextRoomIndex);

        // Set any walls here to 0
        walls[exitLocation.x][exitLocation.y] = 0;
    }

    private void GenerateBarrels()
    {
        // Initialize
        barrels = new List<Entity>();

        // Loop through all tiles
        for (int i = 0; i < floor.Count; i++)
        {
            for (int j = 0; j < floor[i].Count; j++)
            {
                // Get vector
                Vector3Int position = new Vector3Int(i, j);

                // If tile is floor AND is not a wall, or entrance/exit
                if (floor[i][j] == 1 && walls[i][j] == 0 && entranceLocation != position && roomExit.location != position)
                {
                    // Generate barrel on tile by chance
                    if (Random.Range(0, 100) < barrelSpawnChance)
                    {
                        // Make copy
                        var copy = barrel.Copy();
                        // Set room
                        copy.SetRoom(this, position);
                        // Add barrel here
                        barrels.Add(copy);
                    }
                }
            }
        }
    }

    private void GeneratePickups()
    {
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
                Vector3Int position = new Vector3Int(i, j);
                int value = 0;

                // If tile is floor AND is not a wall or barrel or entrance/exit
                if (floor[i][j] == 1 && walls[i][j] == 0 && entranceLocation != position && roomExit.location != position)
                {
                    // Make sure no barrel is here
                    if (barrels.All(barrel => barrel.location != position))
                    {
                        // Generate gold on tile by chance
                        value = Random.Range(0, 100) < goldSpawnChance ? 2 : 0;
                    }
                }

                // Add tile
                pickups[i].Add(value);
            }
        }

        // Randomly spawn keys at the end
        for (int i = 0; i < numKeys; i++)
        {
            // Save location
            Vector3Int location;

            do
            {
                // Generate random point in dungeon
                int randX = Random.Range(padding, width + padding);
                int randY = Random.Range(padding, height + padding);
                location = new Vector3Int(randX, randY);

            } while (!IsValidLocation(location, true));

            // Set key at this point
            pickups[location.x][location.y] = 1;
        }
    }

    public bool IsValidLocation(Vector3Int location, bool ignoreEntity = false)
    {
        // Make sure there is no wall
        if (walls[location.x][location.y] != 0)
        {
            return false;
        }

        // Make sure there is no player or ANY enemy there or ANY barrel
        if (!ignoreEntity)
        {
            if (player.location == location || enemies.Any(enemy => enemy.location == location) || barrels.Any(barrel => barrel.location == location))
            {
                return false;
            }
        }

        // Else you're good to go :)
        return true;
    }

    public bool IsValidPath(Vector3Int start, Vector3Int end, bool ignoreEntity = false, bool endMustBeClear = true)
    {
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

        // Keep looping until start is at the end 
        while (start != end - direction)
        {
            // Check to see if the location is valid
            if (!IsValidLocation(start + direction, ignoreEntity))
            {
                return false;
            }

            // Increment start
            start += direction;
        }

        // Check if you need to ignore the end
        if (endMustBeClear)
        {
            // Make sure there is no enemy on the last tile regardless of conditions
            if (!IsValidLocation(end))
            {
                return false;
            }
        }
        // Check end normally
        else if (!IsValidLocation(end, ignoreEntity))
        {
            return false;
        }


        // Else we good :)
        return true;
    }

    public void UseKey()
    {
        // For each exit, use key
        roomExit.UseKey();
    }
}
