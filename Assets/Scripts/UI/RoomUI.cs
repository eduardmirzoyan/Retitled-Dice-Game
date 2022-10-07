using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Tilemap decorTilemap;
    [SerializeField] private Transform entityTransform;

    [Header("Data")]
    [SerializeField] private Room room;
    [SerializeField] private RuleTile floorTile;
    [SerializeField] private RuleTile wallTile;
    [SerializeField] private Tile entranceTile;
    [SerializeField] private GameObject floorExitPrefab;
    [SerializeField] private GameObject goldPickupPrefab;
    [SerializeField] private GameObject keyPickupPrefab;
    [SerializeField] private GameObject entityModelPrefab;

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
        GameEvents.instance.onEnterFloor += SpawnRoom;
        GameEvents.instance.onSpawnEntity += SpawnEntity;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= SpawnRoom;
        GameEvents.instance.onSpawnEntity -= SpawnEntity;
    }

    // private void Update()
    // {
    //     // For debugging
    //     // Left click to make selection
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         // Get world position from camera
    //         Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         pos.z = 0; // Because camera is -10 from the world
    //         Vector3Int worldPos = floorTilemap.WorldToCell(pos);
    //         // Display pos
    //         print(worldPos);
    //     }
    // }

    private void SpawnRoom(Room room)
    {
        this.room = room;

        // Draw dungeon
        if (room != null)
        {
            Vector3Int position;

            // Loop through all spaces in the dungeon
            for (int i = 0; i < room.walls.Count; i++)
            {
                for (int j = 0; j < room.walls[i].Count; j++)
                {
                    // Set position being considered
                    position = new Vector3Int(i, j, 0);

                    // Check if floor exists here
                    if (room.floor[i][j] == 1)
                    {
                        // Set tile to floor
                        floorTilemap.SetTile(position, floorTile);
                    }

                    // Check if wall exists here
                    if (room.walls[i][j] == 1)
                    {
                        // Set tile to floor
                        wallsTilemap.SetTile(position, wallTile);
                    }

                    // Check if coin exists here
                    if (room.pickups[i][j] == 2)
                    {
                        // Spawn coin
                        var gold = Instantiate(goldPickupPrefab, floorTilemap.GetCellCenterWorld(position), Quaternion.identity, decorTilemap.transform).GetComponent<GoldPickup>();
                        gold.Initialize(position);
                    }

                    // Check if key exists here
                    if (room.pickups[i][j] == 1)
                    {
                        // Spawn key
                        var key = Instantiate(keyPickupPrefab, floorTilemap.GetCellCenterWorld(position), Quaternion.identity, decorTilemap.transform).GetComponent<KeyPickup>();
                        key.Initialize(position);
                    }
                }
            }

            // Spawn all barrels
            foreach (var barrel in room.barrels)
            {
                SpawnEntity(barrel);
            }

            // Spawn entrance
            decorTilemap.SetTile(room.entranceLocation, entranceTile);

            // Spawn exit
            var exit = Instantiate(floorExitPrefab, floorTilemap.GetCellCenterWorld(room.roomExit.location), Quaternion.identity, floorTilemap.transform).GetComponent<RoomExitUI>();
            exit.Initialize(room.roomExit);

            // Get center of dungeon
            Vector3 center = floorTilemap.CellToWorld(new Vector3Int(room.width / 2 + room.padding, room.height / 2 + room.padding, 0));
            // Set camera to center of dungeon
            CameraManager.instance.SetPosition(center);
        }
    }

    private void SpawnEntity(Entity entity)
    {
        // Get world position
        Vector3 worldLocation = floorTilemap.GetCellCenterWorld(entity.location);
        // Spawn model in container
        var model = Instantiate(entityModelPrefab, worldLocation, Quaternion.identity, entityTransform).GetComponent<EntityModel>();
        // Initialize
        model.Initialize(entity, this);
    }

    public Vector3 GetLocationCenter(Vector3Int location)
    {
        // Temp convert function
        return floorTilemap.GetCellCenterWorld(location);
    }
}
