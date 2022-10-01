using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isHitFlash = true;
    public bool isHitFreeze = true;
    public bool isScreenShake = true;
    public bool isSlashEffect = true;
    public bool isHitEffect = true;

    [Header("Entity Data")]
    [SerializeField] private Dungeon dungeon;
    [SerializeField] private EnemyGenerator enemyGenerator;

    [Header("Data")]
    [SerializeField] private int roundNumber = 0;
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
    }

    private IEnumerator EnterFloor()
    {
        // Start the game
        roundNumber = 1;

        // Generate the dungeon
        yield return GenerateDungeon();

        // Generate player and add to dungeon
        yield return GeneratePlayer();

        // Generate enemies and add to dungeon
        yield return GenerateEnemies();

        // Generate pickups
        dungeon.GeneratePickups();

        // Trigger event
        GameEvents.instance.TriggerOnEnterFloor(dungeon);

        // Open scene on player
        var location = DungeonUI.instance.GetLocationCenter(dungeon.player.location);
        TransitionManager.instance.OpenScene(location);

        // Start the first round
        yield return StartRound();
    }

    private IEnumerator GenerateDungeon()
    {
        // Create Dungeon
        dungeon = ScriptableObject.CreateInstance<Dungeon>();
        // Initialize
        dungeon.Initialize(dungeonWidth, dungeonHeight, dungeonPadding, DataManager.instance.GetFloor());

        // Finish
        yield return null;
    }

    private IEnumerator GenerateEnemies()
    {
        // Spawn enemies equal to floor number
        for (int i = 0; i < DataManager.instance.GetFloor(); i++)
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

    private IEnumerator StartRound() {
        // Increment round
        roundNumber++;

        // Reroll all die
        yield return ResetAllDie();

        // Generate turn order
        // Need to make queue to decide enemy turn order
        yield return GenerateTurnQueue();

        // Start turn
        yield return StartTurn();
    }

    private IEnumerator ResetAllDie() {
        // Loop through all entities in dungeon
        foreach (var entity in dungeon.GetAllEntities()) {
            // Loop through all actions
            foreach (var action in entity.actions)
            {
                // Replenish die with event
                action.die.Replenish();
                GameEvents.instance.TriggerOnDieReplenish(action.die);

                // Roll die with event
                action.die.Roll();
                GameEvents.instance.TriggerOnDieRoll(action.die);
            }
        }

        yield return null;
    }

    private IEnumerator StartTurn()
    {
        // Remove first entity from queue
        selectedEntity = turnQueue.Dequeue();

        // If the entity is dead, skip
        if (selectedEntity.currentHealth == 0) {
            print("Entity: " + selectedEntity.name + " is dead so skipping turn.");

            // End turn
            yield return EndTurn();
        }

        // Debug
        print("Turn Start: " + selectedEntity.name);

        // Trigger event 
        GameEvents.instance.TriggerOnTurnStart(selectedEntity);

        // Check if the enity has an ai, if so, let them decide their action
        if (selectedEntity.AI != null)
        {
            // Get best choice
            (Action, Vector3Int) bestChoicePair = selectedEntity.AI.GenerateBestDecision(selectedEntity, dungeon);

            if (bestChoicePair.Item2.z != -1) {
                // Debug
                print("Entity: " + selectedEntity.name + " used: " + bestChoicePair.Item1.name + " on location: " + bestChoicePair.Item2);

                // Select Action
                SelectAction(bestChoicePair.Item1);

                // Select Location
                yield return ConfirmLocationAI(bestChoicePair.Item2);
            }
            else {
                print("Entity: " + selectedEntity.name + " did not perform an action.");
            }

            // End Turn
            yield return EndTurn();
        }
        else {
            // Do nothing
            yield return null;
        }
        
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
        // if (coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(PerformSelectedAction());
    }

    private IEnumerator ConfirmLocationAI(Vector3Int location) {
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
        // if (coroutine != null) StopCoroutine(coroutine);

        yield return PerformSelectedAction();
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

        // Trigger event
        GameEvents.instance.TriggerOnTurnEnd(selectedEntity);

        // Check if queue is empty
        if (turnQueue.Count == 0)
        {
            // Debug
            print("Empty queue, new round: " + roundNumber + 1);

            // Make new round
            yield return StartRound();
        }
        else {
            // Start new turn
            yield return StartTurn();
        }
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

    public void TravelToNextFloor()
    {
        // Exit current floor
        ExitFloor();

        // Increment floor number
        DataManager.instance.IncrementFloor();

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
