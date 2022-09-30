using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private List<Entity> enemies;
    [SerializeField] private Dungeon dungeon;

    [Header("Temp Shit?")]
    [SerializeField] private EnemyGenerator enemyGenerator;
    [SerializeField] private AI enemyAI;

    [Header("Data")]
    [SerializeField] private int dungeonWidth = 12;
    [SerializeField] private int dungeonHeight = 12;
    [SerializeField] private int dungeonPadding = 6;
    [SerializeField] private int numberOfEnemies = 0;
    [SerializeField] private int playerStartingHealth = 3;

    [SerializeField] private Queue<Entity> turnQueue;
    [SerializeField] private int roundNumber;

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

        // Make a copy of the player
        player = (Player) player.Copy();

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

    private IEnumerator StartGame()
    {
        // Start the game
        roundNumber = 1;

        // Generate the dungeon
        yield return GenerateDungeon();

        // Generate enemies and add to dungeon
        yield return GenerateEnemies();

        // Generate player and add to dungeon
        yield return GeneratePlayer();

        // Need to make queue to decide enemy turn order
        yield return GenerateTurnQueue();

        // Trigger event
        GameEvents.instance.TriggerOnEnterFloor(dungeon);

        // Start the first turn
        yield return StartTurn();
    }

    private IEnumerator GenerateDungeon()
    {
        // Create Dungeon
        dungeon = ScriptableObject.CreateInstance<Dungeon>();
        // Initialize
        dungeon.Initialize(dungeonWidth, dungeonHeight, dungeonPadding);

        // Finish
        yield return null;
    }

    private IEnumerator GenerateEnemies() {
        // Initialize list
        enemies = new List<Entity>();
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Generate a random enemy
            var enemy = enemyGenerator.GenerateEnemy();

            // Save it
            enemies.Add(enemy);

            // Populate
            dungeon.Populate(enemy);

            // Trigger event
            GameEvents.instance.TriggerOnGenerateEnity(enemy);
        }

        // Finish
        yield return null;
    }

    private IEnumerator GeneratePlayer() {
        // Populate
        dungeon.Populate(player);

        // Trigger event
        GameEvents.instance.TriggerOnGenerateEnity(player);

        // Finish
        yield return null;
    }

    private IEnumerator GenerateTurnQueue() {
        // Initialize
        turnQueue = new Queue<Entity>();

        // First entity is always the player
        turnQueue.Enqueue(player);

        // Now add all the enemies
        foreach (var enemy in enemies) {
            // Add enemy
            turnQueue.Enqueue(enemy);
        }

        // Debug
        string result = "Generated Queue: ";
        foreach (var ent in turnQueue.ToArray()) {
            result += ent.name + " -> ";
        }
        print(result);
        

        // Finish
        yield return null;
    }

    private IEnumerator StartTurn()
    {
        // Remove first entity from queue
        selectedEntity = turnQueue.Dequeue();

        // Debug
        print("Turn Start: " + selectedEntity.name);

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
            // Get best choice
            (Action, Vector3Int) bestChoicePair = enemyAI.GenerateBestDecision(selectedEntity, dungeon);

            // Debug
            print("Enemy: " + selectedEntity.name + " used: " + bestChoicePair.Item1.name + " on location: " + bestChoicePair.Item2);

            // Select Action
            SelectAction(bestChoicePair.Item1);

            // Select Location
            ConfirmLocation(bestChoicePair.Item2);

            // End Turn
            yield return EndTurn();
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

    public void ConfirmLocation(Vector3Int location)
    {
        this.selectedLocation = location;

        // Debug
        print("Location " + location + " was selected.");

        // Exhaust selected die
        selectedAction.die.Exhaust();

        // Trigger event
        GameEvents.instance.TriggerOnDieExhaust(selectedAction.die);

        // Trigger event
        GameEvents.instance.TriggerOnLocationSelect(location);

        // Perform the selected Action
        if (coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(PerformSelectedAction());
    }

    private IEnumerator PerformSelectedAction()
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

    public void EndTurnNow() {
        // End the current turn
        if (coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(EndTurn());
    }

    private IEnumerator EndTurn()
    {
        // Debug
        print("Turn End: " + selectedEntity.name);

        // Check if queue is empty
        if (turnQueue.Count == 0) {
            // Debug
            print("Empty queue, new round: " + roundNumber + 1);

            // Make new queue
            yield return GenerateTurnQueue();

            // Increment round
            roundNumber++;
        }

        // Start new turn
        yield return StartTurn();
    }

    public void ClearDungeon()
    {
        // Stop co-routine
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = null;

        // Delete dungeon
        dungeon = null;

        // Trigger event
        GameEvents.instance.TriggerOnEnterFloor(null);
    }

    public void TravelNextFloor() {
        // Clear current dungeon
        ClearDungeon();

        // Trigger event
        // TODO

        // Reload this scene
        TransitionManager.instance.ReloadScene(player.location);
    }
    
}
