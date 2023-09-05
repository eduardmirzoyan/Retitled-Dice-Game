using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Room : ScriptableObject
{
    [Header("Settings")]
    public int width;
    public int height;
    public int padding;

    public RoomTile[,] tiles;

    public int numKeys;

    public RoomTile entranceTile;
    public RoomTile exitTile;

    public List<Entity> neutralEntities;
    public List<Entity> hostileEntities;
    public List<Entity> alliedEntities;
    public List<Entity> bossEntities;
    public List<Entity> allEntities;
    public Player player;

    public Pathfinder pathfinder;

    public void Initialize(int roomWidth, int roomHeight, int padding, int wallSpawnChance, int chasamSpawnChance)
    {
        // Save dimensions
        this.width = roomWidth;
        this.height = roomHeight;
        this.padding = padding;

        numKeys = 0;

        // Initalize lists
        hostileEntities = new List<Entity>();
        neutralEntities = new List<Entity>();
        alliedEntities = new List<Entity>();
        bossEntities = new List<Entity>();

        allEntities = new List<Entity>();

        // Initalize pathfinder
        pathfinder = new Pathfinder();

        // Generate world tiles
        GenerateTiles(wallSpawnChance, chasamSpawnChance);
    }

    public void SpawnPlayer(Player player)
    {
        this.player = player;

        // Set the player's location to be at dungeon entrance
        player.Initialize(this, entranceTile.location);

        // Update tile
        entranceTile.containedEntity = player;

        // Add to total list
        allEntities.Add(player);

        // Trigger event
        GameEvents.instance.TriggerOnEntitySpawn(player);
    }

    public void SpawnEntity(Entity entity, bool asBoss = false)
    {
        // Get random point in room
        Vector3Int spawnLocation = GetRandomLocation();

        // Intialize entity
        entity.Initialize(this, spawnLocation);

        // Update tile
        tiles[spawnLocation.x, spawnLocation.y].containedEntity = entity;

        switch (entity.AI.alignment)
        {
            case Alignment.Hostile:

                // If there were no enemies before
                if (hostileEntities.Count == 0)
                {
                    // Trigger event
                    GameEvents.instance.TriggerOnCombatEnter();
                }

                // Save to list
                hostileEntities.Add(entity);

                break;
            case Alignment.Neutral:

                // Save to list
                neutralEntities.Add(entity);

                break;
            case Alignment.Allied:

                // Save to list
                alliedEntities.Add(entity);

                break;
        }

        if (asBoss)
        {
            if (bossEntities.Count == 0)
            {
                // Trigger event
                GameEvents.instance.TriggerOnLockExit();
            }

            bossEntities.Add(entity);
        }

        // Add to total list
        allEntities.Add(entity);

        // Trigger event
        GameEvents.instance.TriggerOnEntitySpawn(entity);
    }

    public void DespawnEntity(Entity entity)
    {
        // Stop any enchantments
        entity.Uninitialize();

        // Check if entity is the player
        if (entity == player)
        {
            // Debug
            Debug.Log("GAME OVER!");

            // Trigger event
            GameEvents.instance.TriggerOnGameLose();

            // Stop game
            GameManager.instance.gameOver = true;
        }
        else if (hostileEntities.Remove(entity))
        {
            // If there are no enemies now
            if (hostileEntities.Count == 0)
            {
                // Trigger event
                GameEvents.instance.TriggerOnCombatExit();
            }
        }
        else if (neutralEntities.Remove(entity))
        {
            // Barrel was removed
            Debug.Log("Barel was killed :(");
        }
        else
        {
            // Error
            throw new System.Exception(entity.name + " was attempted to be removed but didn't exist in dungeon.");
        }

        // If all bosses were killed
        if (bossEntities.Remove(entity) && bossEntities.Count == 0)
        {
            // Trigger event
            GameEvents.instance.TriggerOnUnlockExit();
        }

        // Remove from tile
        TileFromLocation(entity.location).containedEntity = null;

        // Remove from list
        allEntities.Add(entity);

        // Trigger event
        GameEvents.instance.TriggerOnEntityDespawn(entity);
    }

    public void MoveEntityToward(Entity entity, Vector3Int direction)
    {
        // Make sure you are only moving up to 1 tile at a time
        if (direction.magnitude > 1)
            throw new System.Exception("DIRECTION MAG is NOT 1");

        // Determine tile of  next location
        var newTile = TileFromLocation(entity.location + direction);

        // Move to new location
        MoveEntity(entity, newTile);
    }

    public void MoveEntityTo(Entity entity, Vector3Int location)
    {
        // Determine tile of next location
        var newTile = TileFromLocation(location);

        // Move to new location
        MoveEntity(entity, newTile);
    }

    public void MoveEntity(Entity entity, RoomTile newTile)
    {
        // Error check
        if (newTile.containedEntity != null)
            throw new System.Exception("TILE IS ALREADY OCCUPIED BY: " + newTile.containedEntity.name);

        // Remove from old tile
        var oldTile = TileFromLocation(entity.location);
        oldTile.containedEntity = null;

        // Set new location
        entity.location = newTile.location;

        // Set to new tile
        newTile.containedEntity = entity;
    }

    private void GenerateTiles(int wallSpawnChance, int chasamSpawnChance)
    {
        // Initialize tiles
        tiles = new RoomTile[width + 2 * padding, height + 2 * padding];

        for (int i = 0; i < width + 2 * padding; i++)
        {
            for (int j = 0; j < height + 2 * padding; j++)
            {
                var location = new Vector3Int(i, j);
                // Create new tile SO
                var tile = ScriptableObject.CreateInstance<RoomTile>();

                if (WithinPadding(location))
                {
                    // Set to wall
                    tile.Initialize(location, TileType.Wall, this);
                }
                else
                {
                    // Randomly set to wall
                    if (Random.Range(0, 100) < wallSpawnChance)
                    {
                        // Set to wall
                        tile.Initialize(location, TileType.Wall, this);
                    }
                    // Randomly set to chasam
                    else if (Random.Range(0, 100) < chasamSpawnChance)
                    {
                        // Set to chasam
                        tile.Initialize(location, TileType.Chasam, this);
                    }
                    else
                    {
                        // Set to floor
                        tile.Initialize(location, TileType.Floor, this);
                    }
                }

                // Add tile to list
                tiles[i, j] = tile;
            }
        }

        // Get all 4 possible corners
        Vector3Int[] possibleEntranceCorners = {new Vector3Int(padding + 1, padding + 1), new Vector3Int(padding + 1, padding + height - 2),
                                                new Vector3Int(padding + width - 2, padding + height - 2), new Vector3Int(padding + width - 2, padding + 1)};

        // Set entrance to a random set of corners
        var entranceLocation = possibleEntranceCorners[Random.Range(0, possibleEntranceCorners.Length)];
        SpawnEntrance(entranceLocation);

        // Set exit based on entrance
        var exitLocation = new Vector3Int(2 * padding + width - entranceLocation.x - 1, 2 * padding + height - entranceLocation.y - 1);
        SpawnExit(exitLocation);
    }

    private void SpawnEntrance(Vector3Int entranceLocation)
    {
        var tile = ScriptableObject.CreateInstance<RoomTile>();
        tile.Initialize(entranceLocation, TileType.Entrance, this);

        entranceTile = tile;
        tiles[entranceLocation.x, entranceLocation.y] = tile;
    }

    private void SpawnExit(Vector3Int exitLocation)
    {
        var tile = ScriptableObject.CreateInstance<RoomTile>();
        tile.Initialize(exitLocation, TileType.Exit, this);

        exitTile = tile;
        tiles[exitLocation.x, exitLocation.y] = tile;
    }

    public void SpawnPickup(PickUpType pickUpType)
    {
        // Find a random valid location in room
        Vector3Int randomLocation = GetRandomLocation();

        // Update tile
        TileFromLocation(randomLocation).containedPickup = pickUpType;

        switch (pickUpType)
        {
            case PickUpType.Gold:

                // Nothing special needs to happen.

                break;
            case PickUpType.Key:

                // If there were no keys, now we need to lock exit
                if (numKeys == 0)
                {
                    // Trigger event
                    GameEvents.instance.TriggerOnLockExit();
                }

                // Increment keys
                numKeys++;

                break;
        }

        // Trigger event
        GameEvents.instance.TriggerOnPickupSpawn(pickUpType, randomLocation);
    }

    private void DespawnPickup(Entity entity, RoomTile tile)
    {
        // Handle any pickups on tile
        switch (tile.containedPickup)
        {
            case PickUpType.Gold:

                // Give gold to entity
                entity.AddGold(1);

                break;
            case PickUpType.Key:

                // Decrement key count
                numKeys = Mathf.Max(numKeys - 1, 0);
                if (numKeys == 0)
                {
                    // Trigger event
                    GameEvents.instance.TriggerOnUnlockExit();
                }

                break;
        }

        // Update tile
        tile.containedPickup = PickUpType.None;

        // Trigger event
        GameEvents.instance.TriggerOnPickupDespawn(tile.location);
    }

    // ~~~~~~~~~~~~~~ HELPER FUNCTIONS ~~~~~~~~~~~~~~

    private bool WithinPadding(Vector3Int location)
    {
        return location.x < padding || location.x >= width + padding || location.y < padding || location.y >= height + padding;
    }

    public Entity GetEntityAtLocation(Vector3Int location)
    {
        if (location.x < 0 || location.x >= 2 * padding + width || location.y < 0 || location.y >= 2 * padding + height)
            throw new System.Exception("INPUT LOCATION OUT OF BOUNSD: " + location);

        return tiles[location.x, location.y].containedEntity;
    }

    public RoomTile TileFromLocation(Vector3Int location)
    {
        return tiles[location.x, location.y];
    }

    private Vector3Int GetRandomLocation()
    {
        Vector3Int spawnLocation;

        // Keep looping until valid point is found
        while (true)
        {
            int randX = padding + Random.Range(0, width);
            int randY = padding + Random.Range(0, height);
            spawnLocation = new Vector3Int(randX, randY);

            var tile = tiles[spawnLocation.x, spawnLocation.y];

            // Check if everything is good
            if (IsAllClear(spawnLocation) && tile.tileType != TileType.Entrance && tile.tileType != TileType.Exit)
            {
                break;
            }
        }

        return spawnLocation;
    }

    public List<Vector3Int> GetNeighbors(Vector3Int location)
    {
        var neighbors = new List<Vector3Int>();

        // Check cardinal directions
        var position = location + Vector3Int.up;
        if (!IsWall(position) && !IsChasam(position) && !HasEntity(position))
        {
            neighbors.Add(position);
        }

        position = location + Vector3Int.right;
        if (!IsWall(position) && !IsChasam(position) && !HasEntity(position))
        {
            neighbors.Add(position);
        }

        position = location + Vector3Int.down;
        if (!IsWall(position) && !IsChasam(position) && !HasEntity(position))
        {
            neighbors.Add(position);
        }

        position = location + Vector3Int.left;
        if (!IsWall(position) && !IsChasam(position) && !HasEntity(position))
        {
            neighbors.Add(position);
        }

        return neighbors;
    }

    public bool IsWall(Vector3Int location)
    {
        return tiles[location.x, location.y].tileType == TileType.Wall;
    }

    public bool IsChasam(Vector3Int location)
    {
        return tiles[location.x, location.y].tileType == TileType.Chasam;
    }

    public bool HasEntity(Vector3Int location)
    {
        return tiles[location.x, location.y].containedEntity != null;
    }

    public bool HasPickup(Vector3Int location)
    {
        return tiles[location.x, location.y].containedPickup != PickUpType.None;
    }

    public bool IsAllClear(Vector3Int location)
    {
        return !IsWall(location) && !IsChasam(location) && !HasEntity(location) && !HasPickup(location);
    }

    public void InteractWithLocation(Entity entity, Vector3Int location)
    {
        var tile = tiles[location.x, location.y];

        // If exit tile then change level
        if (tile.tileType == TileType.Exit && numKeys == 0 && bossEntities.Count == 0)
        {
            // If we are leaving the boss room
            if (DataManager.instance.GetCurrentRoom() == RoomType.Arena)
            {
                // THEN WE HAVE WON THE GAME!
                GameEvents.instance.TriggerOnGameWin();

                // Stop game
                GameManager.instance.gameOver = true;
            }
            else
            {
                // Go to next floor
                GameManager.instance.TravelToNextFloor();
            }

            return;
        }
        else if (tile.containedPickup != PickUpType.None)
        {
            // Despawn any pickups
            DespawnPickup(entity, tile);
        }
    }

    public static int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
