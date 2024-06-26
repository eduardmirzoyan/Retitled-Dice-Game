using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Room : ScriptableObject
{
    [Header("Settings")]
    public int width;
    public int height;
    public float floorChance;

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
    public ProceduralRoomGenerator roomGenerator;

    public void Initialize(int roomWidth, int roomHeight, float floorChance)
    {
        // Save dimensions
        this.width = roomWidth;
        this.height = roomHeight;
        this.floorChance = floorChance;

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
        GenerateTiles();
    }

    private void GenerateTiles()
    {
        roomGenerator = new ProceduralRoomGenerator();
        roomGenerator.Initialize(width, height, floorChance);
        tiles = roomGenerator.GenerateEmptyRoom(this);
    }

    public void SpawnPlayer(Player player)
    {
        this.player = player;

        foreach (var tile in tiles)
        {
            if (tile.tileType == TileType.Entrance)
            {
                // Set the player's location to be at entrance
                player.Initialize(this, tile.location);

                // Update tile
                tile.containedEntity = player;

                // Add to total list
                allEntities.Add(player);

                // Create in world
                RoomManager.instance.SpawnEntity(player);

                // Trigger event
                GameEvents.instance.TriggerOnEntitySpawn(player);

                return;
            }
        }

        throw new System.Exception("ENTRANCE NOT FOUND!");
    }

    public void SpawnEntity(Entity entity, Vector3Int location, bool asBoss = false)
    {
        // Intialize entity
        entity.Initialize(this, location);

        // Update tile
        tiles[location.x, location.y].containedEntity = entity;

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

        // Create in world
        RoomManager.instance.SpawnEntity(entity);

        // Trigger event
        GameEvents.instance.TriggerOnEntitySpawn(entity);
    }

    public void SpawnEntity(Entity entity, bool asBoss = false)
    {
        // Spawn at random point
        Vector3Int spawnLocation = GetRandomLocation();
        SpawnEntity(entity, spawnLocation, asBoss);
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
        }
        else
        {
            // Error
            throw new System.Exception(entity.name + " was attempted to be removed but didn't exist in room.");
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
        allEntities.Remove(entity);

        // Create in world
        RoomManager.instance.DespawnEntity(entity);

        // Trigger event
        GameEvents.instance.TriggerOnEntityDespawn(entity);
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

    public Entity GetEntityAtLocation(Vector3Int location)
    {
        // Check bounds
        if (IsOutOfBounds(location))
            throw new System.Exception("INPUT LOCATION OUT OF BOUNSD: " + location);

        return tiles[location.x, location.y].containedEntity;
    }

    public RoomTile TileFromLocation(Vector3Int location)
    {
        if (IsOutOfBounds(location))
            throw new System.Exception($"Attempted to get out-of-bounds location {location}");

        return tiles[location.x, location.y];
    }

    private Vector3Int GetRandomLocation()
    {
        Vector3Int spawnLocation;

        // Keep looping until valid point is found
        while (true)
        {
            int randX = Random.Range(0, width);
            int randY = Random.Range(0, height);
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
        if (!IsOutOfBounds(position) && !IsWall(position) && !IsChasam(position) && !HasEntity(position))
        {
            neighbors.Add(position);
        }

        position = location + Vector3Int.right;
        if (!IsOutOfBounds(position) && !IsWall(position) && !IsChasam(position) && !HasEntity(position))
        {
            neighbors.Add(position);
        }

        position = location + Vector3Int.down;
        if (!IsOutOfBounds(position) && !IsWall(position) && !IsChasam(position) && !HasEntity(position))
        {
            neighbors.Add(position);
        }

        position = location + Vector3Int.left;
        if (!IsOutOfBounds(position) && !IsWall(position) && !IsChasam(position) && !HasEntity(position))
        {
            neighbors.Add(position);
        }

        return neighbors;
    }

    public bool IsOutOfBounds(Vector3Int location)
    {
        return location.x < 0 || location.x >= width || location.y < 0 || location.y >= height;
    }

    public bool IsWall(Vector3Int location)
    {
        return tiles[location.x, location.y].tileType == TileType.Wall;
    }

    public bool IsChasam(Vector3Int location)
    {
        return tiles[location.x, location.y].tileType == TileType.Chasam;
    }

    public bool IsObsacle(Vector3Int location)
    {
        return IsOutOfBounds(location) || IsWall(location) || IsChasam(location);
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
            if (DataManager.instance.GetCurrentRoom() == RoomType.Boss)
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
}
