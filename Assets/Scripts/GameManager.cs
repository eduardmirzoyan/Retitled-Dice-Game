using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Entity Data")]
    [SerializeField] private Dungeon dungeon;
    [SerializeField] private EnemyGenerator enemyGenerator;

    [Header("Data")]
    [SerializeField] private int roundNumber;
    [SerializeField] private int dungeonWidth = 12;
    [SerializeField] private int dungeonHeight = 12;
    [SerializeField] private int dungeonPadding = 6;
    [SerializeField] private int numberOfEnemies = 0;

    private Queue<Entity> turnQueue;
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
    }

    private void Start()
    {
        // Start the game
        coroutine = StartCoroutine(EnterFloor());

        // Make sure opens on player?
    }

    private IEnumerator EnterFloor()
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

        // Open scene on player
        var location = DungeonUI.instance.GetLocationCenter(dungeon.player.location);
        // print(location);
        TransitionManager.instance.OpenScene(location);

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

    private IEnumerator GenerateEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Generate a random enemy
            var enemy = enemyGenerator.GenerateEnemy();

            // Populate the dungeon
            dungeon.Populate(enemy);
        }

        // Finish
        yield return null;
    }

    private IEnumerator GeneratePlayer()
    {
        // Get player from data
        var player = DataManager.instance.GetPlayer();

        // Populate the dungeon
        dungeon.Populate(player);

        // Trigger event
        GameEvents.instance.TriggerOnGenerateEnity(player);

        // Finish
        yield return null;
    }

    private IEnumerator GenerateTurnQueue()
    {
        // Initialize
        turnQueue = new Queue<Entity>();

        // First entity is always the player
        turnQueue.Enqueue(dungeon.player);

        // Now pull enemes from the dungeon
        foreach (var enemy in dungeon.enemies)
        {
            // Add enemy
            turnQueue.Enqueue(enemy);
        }

        // Debug
        string result = "Generated Queue: ";
        foreach (var ent in turnQueue.ToArray())
        {
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

        // Trigger event 
        GameEvents.instance.TriggerOnTurnStart(selectedEntity);

        // Check if the enity has an ai, if so, let them decide their action
        if (selectedEntity.AI != null)
        {
            // Get best choice
            (Action, Vector3Int) bestChoicePair = selectedEntity.AI.GenerateBestDecision(selectedEntity, dungeon);

            // Debug
            print("Entity: " + selectedEntity.name + " used: " + bestChoicePair.Item1.name + " on location: " + bestChoicePair.Item2);

            // Select Action
            SelectAction(bestChoicePair.Item1);

            // Select Location
            ConfirmLocation(bestChoicePair.Item2);

            // End Turn
            yield return EndTurn();
        }
        else
        { // If it is a player 

            // Relpenish and Roll all the unit's die
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

    public void EndTurnNow()
    {
        // End the current turn
        if (coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(EndTurn());
    }

    private IEnumerator EndTurn()
    {
        // Debug
        print("Turn End: " + selectedEntity.name);

        // Check if queue is empty
        if (turnQueue.Count == 0)
        {
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

    private void ExitFloor()
    {
        // Leave the current floor

        // Stop any coroutine
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = null;

        // Trigger event
        GameEvents.instance.TriggerOnExitFloor(dungeon);
    }

    public void GameOver() {
        // TODO
    }

    public void TravelToNextFloor()
    {
        // Exit current floor
        ExitFloor();

        // Reload this scene on player
        var location = DungeonUI.instance.GetLocationCenter(dungeon.player.location);
        TransitionManager.instance.ReloadScene(location);
    }

    public void ReturnToMainMenu()
    {
        // Exit current floor
        ExitFloor();

        // Load main menu on player
        var location = DungeonUI.instance.GetLocationCenter(dungeon.player.location);
        TransitionManager.instance.LoadMainMenuScene(location);
    }
}
