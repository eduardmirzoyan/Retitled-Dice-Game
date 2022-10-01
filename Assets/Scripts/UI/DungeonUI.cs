using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] public Tilemap floorTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Tilemap decorTilemap;

    [Header("Data")]
    [SerializeField] private Dungeon dungeon;
    [SerializeField] private RuleTile floorTile;
    [SerializeField] private RuleTile wallTile;
    [SerializeField] private Tile entranceTile;
    [SerializeField] private Tile exitTile;
    [SerializeField] private GameObject floorExitPrefab;
    [SerializeField] private GameObject goldPickupPrefab;
    [SerializeField] private GameObject keyPickupPrefab; // ?

    public static DungeonUI instance;
    private void Awake()
    {
        // Singleton Logic
        if (DungeonUI.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += SpawnDungeon;
        GameEvents.instance.onSpawnEntity += SpawnEntity;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= SpawnDungeon;
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

    private void SpawnDungeon(Dungeon dungeon)
    {
        this.dungeon = dungeon;

        // Draw dungeon
        if (dungeon != null)
        {
            Vector3Int position;

            // Loop through all spaces in the dungeon
            for (int i = 0; i < dungeon.walls.Count; i++)
            {
                for (int j = 0; j < dungeon.walls[i].Count; j++)
                {
                    // Set position being considered
                    position = new Vector3Int(i, j, 0);

                    // Check if floor exists here
                    if (dungeon.floor[i][j] == 1)
                    {
                        // Set tile to floor
                        floorTilemap.SetTile(position, floorTile);
                    }

                    // Check if wall exists here
                    if (dungeon.walls[i][j] == 1)
                    {
                        // Set tile to floor
                        wallsTilemap.SetTile(position, wallTile);
                    }

                    // Check if coin exists here
                    if (dungeon.pickups[i][j] == 2)
                    {
                        // Spawn coin
                        var gold = Instantiate(goldPickupPrefab, floorTilemap.GetCellCenterWorld(position), Quaternion.identity).GetComponent<GoldPickup>();
                        gold.Initialize(position);
                    }

                    // Check if key exists here
                    if (dungeon.pickups[i][j] == 1)
                    {
                        // Spawn key
                        var key = Instantiate(keyPickupPrefab, floorTilemap.GetCellCenterWorld(position), Quaternion.identity).GetComponent<KeyPickup>();
                        key.Initialize(position);
                    }
                }
            }

            // Spawn entrance
            decorTilemap.SetTile(dungeon.entranceLocation, entranceTile);

            // Spawn exit
            var exit = Instantiate(floorExitPrefab, floorTilemap.GetCellCenterWorld(dungeon.exitLocation), Quaternion.identity).GetComponent<FloorExit>();
            exit.Initialize(dungeon.exitLocation);

            // Get center of dungeon
            Vector3 center = floorTilemap.CellToWorld(new Vector3Int(dungeon.width / 2 + dungeon.padding, dungeon.height / 2 + dungeon.padding, 0));
            // Set camera to center of dungeon
            CameraManager.instance.SetPosition(center);
        }
    }

    private void SpawnEntity(Entity entity)
    {
        // Get world position
        Vector3 worldLocation = floorTilemap.GetCellCenterWorld(entity.location);
        // Spawn model
        var model = Instantiate(entity.entityModel, worldLocation, Quaternion.identity).GetComponent<EntityModel>();
        // Initialize
        model.Initialize(entity);
    }

    public Vector3 GetLocationCenter(Vector3Int location)
    {
        // Temp convert function
        return floorTilemap.GetCellCenterWorld(location);
    }
}
