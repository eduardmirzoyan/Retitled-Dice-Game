using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralRoomGenerator
{
    private int width;
    private int height;
    private int padding;
    private float floorChance;

    private static Vector3Int[] DIRECTIONS = new Vector3Int[4] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    public void Initialize(int width, int height, int padding, float floorChance)
    {
        this.width = width;
        this.height = height;
        this.padding = padding;
        this.floorChance = floorChance;
    }

    public RoomTile[,] GenerateEmptyRoom(Room room)
    {
        // Initialize tiles
        RoomTile[,] tiles = new RoomTile[width + 2 * padding, height + 2 * padding];

        // Fill room with walls
        FillRoom(tiles, room);

        var startLocation = SetPathways(tiles);
        int[,] map = new int[width + 2 * padding, height + 2 * padding];
        do
        {
            // Randomly set tiles to floor
            GenerateRandomFloors(tiles);

            // Create a dijkstra map from it
            CreateDijkstraMap(startLocation, map, tiles);
        }
        while (!IsValid(map, tiles));

        return tiles;
    }

    private void FillRoom(RoomTile[,] tiles, Room room)
    {
        for (int i = 0; i < width + 2 * padding; i++)
        {
            for (int j = 0; j < height + 2 * padding; j++)
            {
                var location = new Vector3Int(i, j);
                var tile = ScriptableObject.CreateInstance<RoomTile>();

                // If outside bounds
                if (location.x < padding || location.x >= width + padding || location.y < padding || location.y >= height + padding)
                {
                    // Set to wall
                    tile.Initialize(location, TileType.Chasam, room); // FIXME
                }
                else // Else randomly fill with wall or chasam
                {
                    // 50% Chance
                    if (Random.Range(0, 2) == 0)
                    {
                        // Set to wall
                        tile.Initialize(location, TileType.Wall, room);
                    }
                    else
                    {
                        // Set to chasam
                        tile.Initialize(location, TileType.Chasam, room);
                    }
                }

                // Add tile to list
                tiles[i, j] = tile;
            }
        }
    }

    private Vector3Int SetPathways(RoomTile[,] tiles)
    {
        // Get all 4 possible corners
        Vector3Int[] possibleEntranceCorners = {new Vector3Int(padding + 1, padding + 1), new Vector3Int(padding + 1, padding + height - 2),
                                                new Vector3Int(padding + width - 2, padding + height - 2), new Vector3Int(padding + width - 2, padding + 1)};

        // Set entrance to a random set of corners
        var entranceLocation = possibleEntranceCorners[Random.Range(0, possibleEntranceCorners.Length)];
        tiles[entranceLocation.x, entranceLocation.y].tileType = TileType.Entrance;

        // Set exit based on entrance
        var exitLocation = new Vector3Int(2 * padding + width - entranceLocation.x - 1, 2 * padding + height - entranceLocation.y - 1);
        tiles[exitLocation.x, exitLocation.y].tileType = TileType.Exit;

        return entranceLocation;
    }

    private void GenerateRandomFloors(RoomTile[,] tiles)
    {
        // Now only iterate through 
        System.Random rng = new System.Random();
        for (int i = padding; i < width + padding; i++)
        {
            for (int j = padding; j < height + padding; j++)
            {
                // Set to floor
                if (floorChance > rng.NextDouble() && IsImpassable(new Vector3Int(i, j), tiles))
                {
                    tiles[i, j].tileType = TileType.Floor;
                }
            }
        }
    }

    private void CreateDijkstraMap(Vector3Int startLocation, int[,] map, RoomTile[,] tiles)
    {
        bool[,] visited = new bool[width + 2 * padding, height + 2 * padding];
        Queue<Vector3Int> queue = new Queue<Vector3Int>();

        queue.Enqueue(startLocation);
        map[startLocation.x, startLocation.y] = 1;
        visited[startLocation.x, startLocation.y] = true;

        BFS(queue, map, visited, tiles);
    }

    private void BFS(Queue<Vector3Int> queue, int[,] map, bool[,] visited, RoomTile[,] tiles)
    {
        if (queue.Count == 0)
        {
            return;
        }

        var location = queue.Dequeue();
        var depth = map[location.x, location.y];

        foreach (var direction in DIRECTIONS)
        {
            var newLocation = location + direction;

            if (!IsOutOfBounds(location) && !visited[newLocation.x, newLocation.y])
            {
                // If location is a wall
                if (IsImpassable(newLocation, tiles))
                {
                    // Do not explore
                    map[newLocation.x, newLocation.y] = 0;
                }
                else
                {
                    // Explore
                    queue.Enqueue(newLocation);
                    map[newLocation.x, newLocation.y] = depth + 1;
                }

                // Mark as visited
                visited[newLocation.x, newLocation.y] = true;
            }
        }

        BFS(queue, map, visited, tiles);
    }

    private bool IsValid(int[,] map, RoomTile[,] tiles)
    {
        // If ANY tile is both a floor and unreachable, then room is not valid
        for (int i = padding; i < width + padding; i++)
        {
            for (int j = padding; j < height + padding; j++)
            {
                if (map[i, j] == 0 && !IsImpassable(new Vector3Int(i, j), tiles))
                {
                    return false;
                }
            }
        }

        // Else we all good
        return true;
    }

    private bool IsImpassable(Vector3Int location, RoomTile[,] tiles)
    {
        return tiles[location.x, location.y].tileType == TileType.Wall || tiles[location.x, location.y].tileType == TileType.Chasam;
    }

    private bool IsOutOfBounds(Vector3Int location)
    {
        return location.x < padding || location.x >= width + padding || location.y < padding || location.y >= height + padding;
    }
}
