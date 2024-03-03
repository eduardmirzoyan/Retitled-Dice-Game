using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Tilemap decorTilemap;
    [SerializeField] private Transform entityTransform;

    [Header("Tiles")]
    [SerializeField] private Tile floorTile;
    [SerializeField] private RuleTile wallTile;
    [SerializeField] private Tile entranceTile;

    [Header("Prefabs")]
    [SerializeField] private GameObject floorExitPrefab;
    [SerializeField] private GameObject goldPickupPrefab;
    [SerializeField] private GameObject keyPickupPrefab;
    [SerializeField] private GameObject entityModelPrefab;

    public static RoomManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        // Sub
        GameEvents.instance.onGenerateFloor += DrawRoom;
        GameEvents.instance.onPickupSpawn += SpawnPickup;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onGenerateFloor -= DrawRoom;
        GameEvents.instance.onPickupSpawn -= SpawnPickup;
    }

    private void DrawRoom(Room room)
    {
        // Error check
        if (room == null)
            throw new System.Exception("ROOM IS NULL.");

        // Loop through each tile
        foreach (var tile in room.tiles)
        {
            switch (tile.tileType)
            {
                case TileType.Chasam:

                    // Set tile to empty
                    floorTilemap.SetTile(tile.location, null);
                    wallsTilemap.SetTile(tile.location, null);

                    break;
                case TileType.Floor:

                    // Set tile to floor
                    floorTilemap.SetTile(tile.location, floorTile);

                    break;
                case TileType.Wall:

                    // Set tile to wall
                    floorTilemap.SetTile(tile.location, floorTile);
                    wallsTilemap.SetTile(tile.location, wallTile);

                    break;
                case TileType.Entrance:

                    // Set tile to floor
                    floorTilemap.SetTile(tile.location, floorTile);
                    // Draw entrance
                    decorTilemap.SetTile(tile.location, entranceTile);

                    break;
                case TileType.Exit:

                    // Set tile to floor
                    floorTilemap.SetTile(tile.location, floorTile);

                    // Create room exit
                    Instantiate(floorExitPrefab, floorTilemap.GetCellCenterWorld(tile.location), Quaternion.identity, floorTilemap.transform).GetComponent<RoomExitUI>();

                    break;
            }
        }

        // Get center of room
        Vector3 center = floorTilemap.CellToWorld(new Vector3Int(room.width / 2 + room.padding, room.height / 2 + room.padding, 0));
        // Set camera to center of room
        CameraManager.instance.SetPosition(center);
    }

    public void SpawnEntity(Entity entity)
    {
        // Spawn entity in the world
        Vector3 worldLocation = floorTilemap.GetCellCenterWorld(entity.location);
        var entityModel = Instantiate(entityModelPrefab, worldLocation, Quaternion.identity, entityTransform).GetComponent<EntityModel>();
        entityModel.Initialize(entity);

        // Set reference
        entity.model = entityModel;
    }

    public void DespawnEntity(Entity entity)
    {
        // Create a corpse (if not a barrel)
        if (entity.name != "Barrel")
            entity.model.SpawnCorpse();

        // Remove from world
        entity.model.Uninitialize();
        Destroy(entity.model.gameObject);

        // Remove reference
        entity.model = null;
    }

    private void SpawnPickup(PickUpType pickUpType, Vector3Int location)
    {
        switch (pickUpType)
        {
            case PickUpType.None:
                // Do nothing
                break;

            case PickUpType.Gold:

                // Spawn gold coin
                var gold = Instantiate(goldPickupPrefab, floorTilemap.GetCellCenterWorld(location), Quaternion.identity, decorTilemap.transform).GetComponent<GoldPickup>();
                gold.Initialize(location);

                break;
            case PickUpType.Key:

                // Spawn key
                var key = Instantiate(keyPickupPrefab, floorTilemap.GetCellCenterWorld(location), Quaternion.identity, decorTilemap.transform).GetComponent<KeyPickup>();
                key.Initialize(location);

                break;
        }
    }

    public Vector3 GetLocationCenter(Vector3Int location)
    {
        // Temp convert function
        return floorTilemap.GetCellCenterWorld(location);
    }
}
