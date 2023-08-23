using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Tilemap decorTilemap;
    [SerializeField] private Transform entityTransform;
    [SerializeField] private Transform projectilesTransform;

    [Header("Data")]
    [SerializeField] private Room room;
    [SerializeField] private RuleTile floorTile;
    [SerializeField] private RuleTile wallTile;
    [SerializeField] private Tile entranceTile;
    [SerializeField] private GameObject floorExitPrefab;
    [SerializeField] private GameObject goldPickupPrefab;
    [SerializeField] private GameObject keyPickupPrefab;
    [SerializeField] private GameObject entityModelPrefab;
    [SerializeField] private GameObject projectileModelPrefab;

    public static RoomUI instance;
    private void Awake()
    {
        // Singleton Logic
        if (RoomUI.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += DrawRoom;
        GameEvents.instance.onEntitySpawn += SpawnEntity;
        GameEvents.instance.onPickupSpawn += SpawnPickup;
        GameEvents.instance.onProjectileSpawn += SpawnProjectile;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= DrawRoom;
        GameEvents.instance.onEntitySpawn -= SpawnEntity;
        GameEvents.instance.onPickupSpawn += SpawnPickup;
        GameEvents.instance.onProjectileSpawn -= SpawnProjectile;
    }

    private void DrawRoom(Room room)
    {
        // Error check
        if (room == null)
            throw new System.Exception("ROOM IS NULL.");

        this.room = room;

        // Loop through each tile
        foreach (var tile in room.tiles)
        {
            switch (tile.tileType)
            {
                case TileType.Void:

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

                    Instantiate(floorExitPrefab, floorTilemap.GetCellCenterWorld(tile.location), Quaternion.identity, floorTilemap.transform);
                    // exit.Initialize(room.roomExit);

                    break;
            }

            // switch (tile.containedPickup)
            // {
            //     case PickUpType.None:
            //         // Do nothing
            //         break;

            //     case PickUpType.Gold:

            //         // Spawn gold coin
            //         var gold = Instantiate(goldPickupPrefab, floorTilemap.GetCellCenterWorld(tile.location), Quaternion.identity, decorTilemap.transform).GetComponent<GoldPickup>();
            //         gold.Initialize(tile.location);

            //         break;
            //     case PickUpType.Key:

            //         // Spawn key
            //         var key = Instantiate(keyPickupPrefab, floorTilemap.GetCellCenterWorld(tile.location), Quaternion.identity, decorTilemap.transform).GetComponent<KeyPickup>();
            //         key.Initialize(tile.location);

            //         break;
            // }
        }

        // Get center of dungeon
        Vector3 center = floorTilemap.CellToWorld(new Vector3Int(room.width / 2 + room.padding, room.height / 2 + room.padding, 0));
        // Set camera to center of dungeon
        CameraManager.instance.SetPosition(center);
    }

    private void SpawnEntity(Entity entity)
    {
        // Get world position
        Vector3 worldLocation = floorTilemap.GetCellCenterWorld(entity.location);
        // Spawn model in container
        var entityModel = Instantiate(entityModelPrefab, worldLocation, Quaternion.identity, entityTransform).GetComponent<EntityModel>();
        // Initialize
        entityModel.Initialize(entity, this);
    }

    private void SpawnProjectile(Projectil projectil)
    {
        // Get world position
        Vector3 worldLocation = floorTilemap.GetCellCenterWorld(projectil.location);
        // Spawn model in container
        var projModel = Instantiate(projectileModelPrefab, worldLocation, Quaternion.identity, projectilesTransform).GetComponent<ProjectileModel>();
        // Initialize
        projModel.Initialize(projectil, this);
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
