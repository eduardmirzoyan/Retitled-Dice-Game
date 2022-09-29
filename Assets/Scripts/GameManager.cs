using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Dungeon dungeon;

    [Header("Data")]
    [SerializeField] private int dungeonWidth = 12;
    [SerializeField] private int dungeonHeight = 12;
    [SerializeField] private int dungeonPadding = 6;
    [SerializeField] private int numberOfEnemies = 0;
    [SerializeField] private int playerStartingHealth = 3;

    private Entity selectedEntity;
    private Action selectedAction;
    private Vector3Int selectedLocation;

    private Coroutine coroutine;

    public static GameManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    public void CreateNewPlayer()
    {
        // Create player SO
        // This will be done in main menu?

        // Initialize
        player.Initialize(playerStartingHealth);
    }

    private void Update()
    {
        // // Debugging
        if (Input.GetKeyDown(KeyCode.Space) && coroutine == null)
        {
            // Create player
            // CreateNewPlayer();

            // Start the game
            coroutine = StartCoroutine(StartGame());
        }
    }

    public IEnumerator StartGame()
    {
        // Start the game

        // Generate enemies

        // Generate the dungeon and enemies within
        yield return GenerateDungeon();

        // Need to make queue to decide enemy turn order
        // TODO

        // Intialize entities?

        // Temp set entity
        selectedEntity = player;

        // Start the Player Turn
        yield return StartTurn();
    }

    public IEnumerator GenerateDungeon()
    {
        // Create Dungeon
        dungeon = ScriptableObject.CreateInstance<Dungeon>();
        // Initialize with player
        dungeon.Initialize(dungeonWidth, dungeonHeight, dungeonPadding, numberOfEnemies, player);

        // Trigger event
        GameEvents.instance.TriggerOnGenerateDungeon(dungeon);

        // Finish
        yield return null;
    }

    public IEnumerator GenerateEnemies() {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Create enemy   
        }

        // Finish
        yield return null;
    }

    public IEnumerator StartTurn()
    {
        // Check if the enity that taking it's turn is the player
        if (selectedEntity is Player)
        {
            // Relpenish and Roll all die
            foreach (var action in selectedEntity.actions)
            {
                // Replenish die with event
                action.die.Replenish();
                GameEvents.instance.TriggerOnDieReplenish(action.die);

                // Roll die with event
                action.die.Roll();
                GameEvents.instance.TriggerOnDieRoll(action.die);
            }
        }
        else
        {
            // Do AI shit here
            // TODO

            yield return null;
        }


        // Trigger event 
        GameEvents.instance.TriggerOnTurnStart(selectedEntity);

        // Finish
        yield return null;
    }

    public void SelectAction(Action action)
    {
        // Update selected action (could be null)
        selectedAction = action;

        if (action != null)
        {
            // Debug
            print("Action " + action.name + " was selected.");
        }
        else
        {
            // Debug
            print("Action was de-selected.");
        }

        // Trigger event
        GameEvents.instance.TriggerOnActionSelect(selectedEntity, selectedAction, dungeon);
    }

    public void SelectLocation(Vector3Int location)
    {
        this.selectedLocation = location;

        // Debug
        print("Location " + location + " was selected.");

        // Exhaust selected die
        // selectedAction.die.Exhaust();

        // Trigger event
        // GameEvents.instance.TriggerOnDieExhaust(selectedAction.die);

        // Trigger event
        GameEvents.instance.TriggerOnLocationSelect(location);

        // Perform the selected Action
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(PerformSelectedAction());
    }

    public IEnumerator PerformSelectedAction()
    {
        // Trigger event
        GameEvents.instance.TriggerOnActionPerformStart(selectedEntity, selectedAction, selectedLocation, dungeon);

        // Perform the action and wait until it's finished
        yield return selectedAction.Perform(selectedEntity, selectedLocation, dungeon);

        // After the action is performed, re-enable UI
        GameEvents.instance.TriggerOnActionPerformEnd(selectedEntity, selectedAction, selectedLocation, dungeon);

        // Reset selected values
        selectedAction = null;
        selectedLocation = Vector3Int.zero;
    }

    public void EndTurn()
    {
        // TODO

        // Remove selectedEntity from queue
        // Then select next entity and start a new turn
        // If queue is empty, refill it and increment round?
    }

    public void ClearDungeon()
    {
        // Stop co-routine
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = null;

        // Delete dungeon
        dungeon = null;

        // Trigger event
        GameEvents.instance.TriggerOnGenerateDungeon(null);
    }

    public void TravelNextFloor() {
        // Clear current dungeon
        ClearDungeon();

        // Trigger event
        // TODO

        // Reload this scene
        TransitionManager.instance.ReloadScene();
    }
    
}
