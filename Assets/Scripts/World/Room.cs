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
    public Player player;

    public Pathfinder pathfinder;

    public void Initialize(int roomWidth, int roomHeight, int padding, int wallSpawnChance)
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

        // Initalize pathfinder
        pathfinder = new Pathfinder();

        // Generate world tiles
        GenerateTiles(wallSpawnChance);
    }

    public void SpawnPlayer(Player player)
    {
        this.player = player;

        // Set the player's location to be at dungeon entrance
        player.Initialize(this, entranceTile.location);

        // Update tile
        entranceTile.containedEntity = player;

        // Trigger event
        GameEvents.instance.TriggerOnEntitySpawn(player);
    }

    public void SpawnEntity(Entity entity)
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

                // Save to list
                hostileEntities.Add(entity);

                // Check to lock inventory?

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

        // Trigger event
        GameEvents.instance.TriggerOnEntitySpawn(entity);
    }

    public void DespawnEntity(Entity entity)
    {
        // Check if entity is the player
        if (entity == player)
        {
            // Debug
            Debug.Log("GAME OVER!");

            // Trigger event?
        }
        else if (hostileEntities.Remove(entity))
        {
            // Enemy was removed
        }
        else if (neutralEntities.Remove(entity))
        {
            // Barrel was removed
        }
        else
        {
            // Error
            throw new System.Exception(entity.name + " was attempted to be removed but didn't exist in dungeon.");
        }

        // Remove from tile
        TileFromLocation(entity.location).containedEntity = null;

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

        // Trigger event
        GameEvents.instance.TriggerOnEntityEnterTile(entity, entity.location);
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

    private void GenerateTiles(int wallSpawnChance)
    {
        tiles = new RoomTile[width + 2 * padding, height + 2 * padding];

        // Get all 4 possible corners
        Vector3Int[] possibleEntranceCorners = {new Vector3Int(padding + 1, padding + 1), new Vector3Int(padding + 1, padding + height - 2),
                                                new Vector3Int(padding + width - 2, padding + height - 2), new Vector3Int(padding + width - 2, padding + 1)};

        // Set entrance to a random set of corners
        var entranceLocation = possibleEntranceCorners[Random.Range(0, possibleEntranceCorners.Length)];

        // Set exit based on entrance
        var exitLocation = new Vector3Int(2 * padding + width - entranceLocation.x - 1, 2 * padding + height - entranceLocation.y - 1);

        for (int i = 0; i < width + 2 * padding; i++)
        {
            for (int j = 0; j < height + 2 * padding; j++)
            {
                // Create new tile SO
                var tile = ScriptableObject.CreateInstance<RoomTile>();
                var location = new Vector3Int(i, j);

                // Initialize it to default
                tile.Initialize(location, this);

                TrySetPadding(tile);

                TrySpawnEntrance(tile, entranceLocation);

                TrySpawnExit(tile, exitLocation);

                TrySpawnWall(tile, wallSpawnChance);

                // Add tile to list
                tiles[i, j] = tile;
            }
        }
    }

    private void TrySetPadding(RoomTile tile)
    {
        // If you are in the padding range then add wall
        if (tile.location.x < padding || tile.location.x >= width + padding || tile.location.y < padding || tile.location.y >= height + padding)
        {
            tile.tileType = TileType.Wall;
        }
    }

    private void TrySpawnEntrance(RoomTile tile, Vector3Int entranceLocation)
    {
        if (tile.location != entranceLocation)
            return;

        tile.tileType = TileType.Entrance;
        entranceTile = tile;
    }

    private void TrySpawnExit(RoomTile tile, Vector3Int exitLocation)
    {
        if (tile.location != exitLocation)
            return;

        tile.tileType = TileType.Exit;
        exitTile = tile;
    }

    private void TrySpawnWall(RoomTile tile, int wallSpawnChance)
    {
        // Do nothing if not a floor tile
        if (tile.tileType != TileType.Floor)
            return;

        bool flag = Random.Range(0, 100) < wallSpawnChance;
        if (flag)
        {
            tile.tileType = TileType.Wall;
        }
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

    private Vector3Int GetRandomLocation()
    {
        Vector3Int spawnLocation;

        // Keep looping until valid point is found
        while (true)
        {
            int randX = padding + Random.Range(0, width);
            int randY = padding + Random.Range(0, height);
            spawnLocation = new Vector3Int(randX, randY);

            // Check if everything is good
            if (IsValidLocation(spawnLocation, false, false) && (spawnLocation != entranceTile.location || spawnLocation != exitTile.location))
            {
                break;
            }

        }

        return spawnLocation;
    }

    public List<Vector3Int> GetNeighbors(Vector3Int location, bool ignoreEntity = false)
    {
        var neighbors = new List<Vector3Int>();

        // Check cardinal directions
        var position = location + Vector3Int.up;
        if (IsValidLocation(position, ignoreEntity))
        {
            neighbors.Add(position);
        }

        position = location + Vector3Int.right;
        if (IsValidLocation(position, ignoreEntity))
        {
            neighbors.Add(position);
        }

        position = location + Vector3Int.down;
        if (IsValidLocation(position, ignoreEntity))
        {
            neighbors.Add(position);
        }

        position = location + Vector3Int.left;
        if (IsValidLocation(position, ignoreEntity))
        {
            neighbors.Add(position);
        }

        return neighbors;
    }

    public bool IsValidLocation(Vector3Int location, bool ignoreEntity = false, bool ignorePickup = true)
    {
        // Debug.Log(location);

        var tile = TileFromLocation(location);

        // Make sure there is no wall or void
        if (tile.tileType == TileType.Wall || tile.tileType == TileType.Void)
        {
            return false;
        }

        // Make sure there is no entity
        if (!ignoreEntity)
        {
            // If there is an entity on this tile, dip
            if (tile.containedEntity != null)
                return false;
        }

        if (!ignorePickup)
        {
            // If there is a pickup
            if (tile.containedPickup != PickUpType.None)
                return false;
        }

        // Else you're good to go :)
        return true;
    }

    public bool IsValidPath(Vector3Int start, Vector3Int end, bool ignoreEntity = false, bool endMustBeClear = true)
    {
        // Get path between start and end
        Vector3Int direction = end - start;

        // Make sure path is straight
        if (!(direction.x == 0 || direction.y == 0))
        {
            return false;
        }

        // Normalize direction
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        // Debug
        Debug.Log("Direction: " + direction);

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
            if (!IsValidLocation(end, ignoreEntity))
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

    public Vector3Int GetFirstValidLocationWithinRange(Vector3Int start, Vector3Int direction, int range)
    {
        while (IsValidLocation(start + direction, true) && range > 0)
        {
            start += direction;
            range--;
        }

        return start;
    }

    public List<Vector3Int> GetAllValidLocationsAlongPath(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        Vector3Int direction = end - start;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        while (start != end)
        {
            // Check to see if the location is valid
            if (!IsValidLocation(start, true))
            {
                break;
            }

            result.Add(start + direction);

            // Increment start
            start += direction;
        }

        return result;
    }

    public void InteractWithLocation(Entity entity, Vector3Int location)
    {
        var tile = tiles[location.x, location.y];

        // If exit tile then change level
        if (tile.tileType == TileType.Exit && numKeys == 0)
        {
            // Go to next floor
            GameManager.instance.TravelToNextFloor();

            return;
        }
        else if (tile.containedPickup != PickUpType.None)
        {
            // Despawn any pickups
            DespawnPickup(entity, tile);
        }

    }

    public bool HasHostileEntities()
    {
        return hostileEntities.Count > 0;
    }
}
