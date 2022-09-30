using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonUI : MonoBehaviour
{
    public static DungeonUI instance;

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

    private void Awake() {
        // Singleton Logic
        if (DungeonUI.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start() {
        // Sub
        GameEvents.instance.onEnterFloor += DrawDungeon;
        GameEvents.instance.onGenerateEntity += SpawnEntity;
    }

    private void OnDestroy() {
        // Unsub
        GameEvents.instance.onEnterFloor -= DrawDungeon;
        GameEvents.instance.onGenerateEntity -= SpawnEntity;
    }

    private void Update()
    {
        // For debugging
        // Left click to make selection
        if (Input.GetMouseButtonDown(0))
        {
            // Get world position from camera
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0; // Because camera is -10 from the world
            Vector3Int worldPos = floorTilemap.WorldToCell(pos);
            // Display pos
            // print(worldPos);
        }
    }

    private void DrawDungeon(Dungeon dungeon) {
        this.dungeon = dungeon;

        // Draw dungeon
        if (dungeon != null) {
            // Sub to events

            // Loop through all spaces in the dungeon
            for (int i = 0; i < dungeon.walls.Count; i++)
            {
                for (int j = 0; j < dungeon.walls[i].Count; j++)
                {
                    // Check if floor exists here
                    if (dungeon.floor[i][j] == 1)
                    {
                        // Set tile to floor
                        floorTilemap.SetTile(new Vector3Int(i, j, 0), floorTile);
                    }

                    // Check if wall exists here
                    if (dungeon.walls[i][j] == 1)
                    {
                        // Set tile to floor
                        wallsTilemap.SetTile(new Vector3Int(i, j, 0), wallTile);
                    }
                }
            }

            // Draw entrance TODO
            // decorTilemap.SetTile(dungeon.entranceLocation, entranceTile);

            // Draw exit
            decorTilemap.SetTile(dungeon.exitLocation, exitTile);

            // Get center of dungeon
            Vector3 center = floorTilemap.CellToWorld(new Vector3Int(dungeon.width / 2 + dungeon.padding, dungeon.height / 2 + dungeon.padding, 0));
            // Set camera to center of dungeon
            CameraManager.instance.SetPosition(center);
        }
        // Else clear any drawings
        else {
            // floorTilemap.ClearAllTiles();
            // wallsTilemap.ClearAllTiles();

            // Unsub to events
            GameEvents.instance.onEnterFloor -= DrawDungeon;
            GameEvents.instance.onGenerateEntity -= SpawnEntity;
        }
        
    }

    private void SpawnEntity(Entity entity) {
        // Spawn entity model
        Vector3 worldLocation = floorTilemap.GetCellCenterWorld(entity.location);
        var model = Instantiate(entity.entityModel, worldLocation, Quaternion.identity).GetComponent<EntityModel>();
        // Initialize
        model.Initialize(entity);
    }
}
