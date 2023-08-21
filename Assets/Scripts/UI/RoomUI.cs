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
    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private Transform entityTransform;
    [SerializeField] private Transform projectilesTransform;

    [Header("Data")]
    [SerializeField] private Room room;
    [SerializeField] private RuleTile floorTile;
    [SerializeField] private RuleTile wallTile;
    [SerializeField] private Tile entranceTile;
    [SerializeField] private AnimatedTile selectionTile;
    [SerializeField] private GameObject floorExitPrefab;
    [SerializeField] private GameObject goldPickupPrefab;
    [SerializeField] private GameObject keyPickupPrefab;
    [SerializeField] private GameObject entityModelPrefab;
    [SerializeField] private GameObject projectileModelPrefab;
    [SerializeField] private Vector3Int selectedLocation;

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

        // Set default inspect
        selectedLocation = Vector3Int.back;
    }

    private void Start()
    {
        // Sub
        GameEvents.instance.onEnterFloor += SpawnRoom;
        GameEvents.instance.onEntitySpawn += SpawnEntity;
        GameEvents.instance.onProjectileSpawn += SpawnProjectile;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onEnterFloor -= SpawnRoom;
        GameEvents.instance.onEntitySpawn -= SpawnEntity;
        GameEvents.instance.onProjectileSpawn -= SpawnProjectile;
    }

    private void Update()
    {
        // Left click to make selection
        if (Input.GetMouseButtonDown(0))
        {
            // Get world position from camera
            Vector3 cameraLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cameraLocation.z = 0; // Because camera is -10 from the world
            Vector3Int selectedLocation = selectionTilemap.WorldToCell(cameraLocation);
            // Select position
            SelectTile(selectedLocation);
            // Debug
            // print(selectedLocation);
        }
        // Right click clears any selection
        else if (Input.GetMouseButtonDown(1))
        {
            // Clear selection
            SelectTile(Vector3Int.back);
        }
    }

    public void SelectTile(Vector3Int location)
    {
        // If selected tile is not undefined 
        if (selectedLocation != Vector3Int.back)
        {
            // Clear last selection
            selectionTilemap.SetTile(selectedLocation, null);
        }

        // If new location isn't empty
        if (location != Vector3Int.back)
        {
            // Set tile
            selectionTilemap.SetTile(location, selectionTile);

            // Inspect tile
            InspectLocation(location);
        }
        else
        {
            // Remove selection
            selectionTilemap.SetTile(location, null);
            // Deselect
            InspectLocation(location);
        }

        // Set new location
        this.selectedLocation = location;
    }

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

    private void InspectLocation(Vector3Int location)
    {
        // Look for enemy on this tile
        foreach (var enemy in room.enemies)
        {
            // If an enemy exists at this location
            if (enemy.location == location)
            {
                // Trigger inspect event
                GameEvents.instance.TriggerOnEntityInspect(enemy);
                // Finish
                return;
            }
        }

        // Look at barrels
        foreach (var barrel in room.barrels)
        {
            // If an enemy exists at this location
            if (barrel.location == location)
            {
                // Trigger inspect event
                GameEvents.instance.TriggerOnEntityInspect(barrel);
                // Finish
                return;
            }
        }

        // Else if enemy was not found, then remove any previous inspect
        GameEvents.instance.TriggerOnEntityInspect(null);
    }

    public Vector3 GetLocationCenter(Vector3Int location)
    {
        // Temp convert function
        return floorTilemap.GetCellCenterWorld(location);
    }
}
