using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralRoomGenerator
{
    private int width;
    private int height;
    private float floorChance;

    private static Vector3Int[] DIRECTIONS = new Vector3Int[4] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    public void Initialize(int width, int height, float floorChance)
    {
        this.width = width;
        this.height = height;
        this.floorChance = floorChance;
    }

    public RoomTile[,] GenerateEmptyRoom(Room room)
    {
        // Initialize tiles
        RoomTile[,] tiles = new RoomTile[width, height];

        // Fill room with walls
        FillRoom(tiles, room);

        var startLocation = SetPathways(tiles);
        int[,] map = new int[width, height];
        do
        {
            // Randomly set tiles to floor
            GenerateRandomFloors(tiles);

            // Create a dijkstra map from it
            CreateDijkstraMap(startLocation, map, tiles);
        }
        while (!IsValidMap(map, tiles));

        return tiles;
    }

    private void FillRoom(RoomTile[,] tiles, Room room)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var location = new Vector3Int(i, j);
                var tile = ScriptableObject.CreateInstance<RoomTile>();

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

                // Add tile to list
                tiles[i, j] = tile;
            }
        }
    }

    private Vector3Int SetPathways(RoomTile[,] tiles)
    {
        // Get all 4 possible corners
        Vector3Int[] possibleEntranceCorners = {new Vector3Int(1, 1), new Vector3Int(1, height - 2),
                                                new Vector3Int(width - 2, height - 2), new Vector3Int(width - 2, 1)};

        // Set entrance to a random set of corners
        var entranceLocation = possibleEntranceCorners[Random.Range(0, possibleEntranceCorners.Length)];
        tiles[entranceLocation.x, entranceLocation.y].tileType = TileType.Entrance;

        // Set exit based on entrance
        var exitLocation = new Vector3Int(width - entranceLocation.x - 1, height - entranceLocation.y - 1);
        tiles[exitLocation.x, exitLocation.y].tileType = TileType.Exit;

        return entranceLocation;
    }

    private void GenerateRandomFloors(RoomTile[,] tiles)
    {
        // Now only iterate through 
        System.Random rng = new System.Random();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
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
        bool[,] visited = new bool[width, height];
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

            if (!IsOutOfBounds(newLocation) && !visited[newLocation.x, newLocation.y])
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

    private bool IsValidMap(int[,] map, RoomTile[,] tiles)
    {
        // If ANY tile is both a floor and unreachable, then room is not valid
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
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
        return location.x < 0 || location.x >= width || location.y < 0 || location.y >= height;
    }
}
