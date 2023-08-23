using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isHitFlash = true;
    public bool isHitFreeze = true;
    public bool isScreenShake = true;
    public bool isSlashEffect = true;
    public bool isHitEffect = true;

    [Header("Cursor")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    [Header("Entity Data")]
    [SerializeField] private Room room;
    [SerializeField] private RoomGenerator roomGenerator;
    [SerializeField] private EnemyGenerator enemyGenerator;

    [Header("Data")]
    [SerializeField] private int roundNumber = 0;
    [SerializeField] public float bufferTime = 0.5f;

    private Queue<Entity> turnQueue;
    private Entity selectedEntity;
    private Action selectedAction;
    private Vector3Int selectedLocation;
    private List<(Action, Vector3Int)> bestChoiceSequence;
    private Dictionary<Action, List<Vector3Int>> threatenLocationsHashtable;
    private Dictionary<Entity, (Action, Vector3Int)> delayedActionsHashtable;
    private Dictionary<Vector3Int, List<(Entity, Action)>> reactiveActionsHastable;


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

        bestChoiceSequence = new List<(Action, Vector3Int)>();

        threatenLocationsHashtable = new Dictionary<Action, List<Vector3Int>>();
        reactiveActionsHastable = new Dictionary<Vector3Int, List<(Entity, Action)>>();

        delayedActionsHashtable = new Dictionary<Entity, (Action, Vector3Int)>();
    }

    private void Start()
    {
        // Start the game
        coroutine = StartCoroutine(EnterFloor());
    }

    private IEnumerator EnterFloor()
    {
        // Reset round count
        roundNumber = 1;

        // Generate the room
        yield return GenerateRoom();

        // Generate Keys
        yield return GenerateKeys();

        // Generate coins?
        // TODO

        // Generate player and add to room
        yield return GeneratePlayer();

        // Generate enemies and add to room
        yield return GenerateEnemies();

        // Trigger event
        GameEvents.instance.TriggerOnEnterFloor(room);

        // Open scene on player
        var location = RoomUI.instance.GetLocationCenter(room.player.location);
        TransitionManager.instance.OpenScene(location);

        // Start the first round
        yield return StartRound();
    }

    private IEnumerator GenerateRoom()
    {
        // Use the room index to decide what room to generate
        int index = DataManager.instance.GetRoomIndex();
        switch (index)
        {
            case 1:
                // Generate a normal room
                room = roomGenerator.GenerateRoom();
                break;
            case -1:
                // Generate a shop
                room = roomGenerator.GenerateShop();
                break;
            default:
                throw new System.Exception("ERROR CREATING ROOM WITH ROOM INDEX: " + index);
        }

        // Finish
        yield return null;
    }

    private IEnumerator GenerateEnemies()
    {
        int index = DataManager.instance.GetRoomIndex();
        switch (index)
        {
            case 1:
                // Spawn enemies equal to floor number
                for (int i = 0; i < DataManager.instance.GetRoomNumber(); i++)
                {
                    // Generate a random enemy
                    var enemy = enemyGenerator.GenerateEnemy();

                    // Populate the room
                    room.SpawnEntity(enemy);
                }

                break;
            case -1:
                // REDO THIS LATER

                // Generate a shopkeeper
                var shopkeeper = enemyGenerator.GenerateShopkeeper();

                // Populate the room
                room.SpawnEntity(shopkeeper);

                break;
            default:
                throw new System.Exception("ERROR CREATING ENEMIES WITH ROOM INDEX: " + index);
        }

        // Finish
        yield return null;
    }

    private IEnumerator GeneratePlayer()
    {
        // Get player from data
        var player = DataManager.instance.GetPlayer();

        // Populate the room
        room.SpawnEntity(player);

        // Finish
        yield return null;
    }

    private IEnumerator GenerateKeys()
    {
        // Create keys based on room number
        for (int i = 0; i < DataManager.instance.GetRoomNumber(); i++)
        {
            room.AddKey();
        }

        // Finish
        yield return null;
    }

    private IEnumerator GenerateTurnQueue()
    {
        // Initialize
        turnQueue = new Queue<Entity>();

        // First entity is always the player
        turnQueue.Enqueue(room.player);

        // Now pull enemes from the room
        foreach (var enemy in room.hostileEntities)
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
        result += "END";
        print(result);


        // Finish
        yield return null;
    }

    private IEnumerator StartRound()
    {
        // Increment round
        roundNumber++;

        // Generate turn order
        // Need to make queue to decide enemy turn order
        yield return GenerateTurnQueue();

        // Start turn
        yield return StartTurn();
    }

    private void ResetActions(Entity entity)
    {
        // Loop through all actions
        foreach (var action in entity.GetActions())
        {
            // Replenish die with event
            action.die.Replenish();
            GameEvents.instance.TriggerOnDieReplenish(action.die);

            // Roll die with event
            action.die.Roll();
            GameEvents.instance.TriggerOnDieRoll(action.die);
        }
    }

    private IEnumerator StartTurn()
    {
        // Remove first entity from queue
        selectedEntity = turnQueue.Dequeue();

        // If the entity is dead, skip
        if (selectedEntity.currentHealth == 0)
        {
            print("Entity: " + selectedEntity.name + " is dead so skipping turn.");

            // End turn
            yield return EndTurn();
        }

        // Debug
        print("Turn Start: " + selectedEntity.name);

        // Trigger event 
        GameEvents.instance.TriggerOnTurnStart(selectedEntity);

        // Perform any delayed actions stored by this entity
        yield return PerformDelayedAction(selectedEntity);

        // Reset actions now
        ResetActions(selectedEntity);

        // Check if the enity has an ai, if so, let them decide their action
        if (selectedEntity.AI != null)
        {
            // Give small pause
            yield return new WaitForSeconds(bufferTime);

            // Get best sequence of actions
            bestChoiceSequence = selectedEntity.AI.GenerateSequenceOfActions(selectedEntity, room, room.player);

            // Then start performing those actions
            yield return PerformEnemyTurn();
        }
        else
        {
            // Do nothing
            yield return null;
        }
    }

    private IEnumerator PerformEnemyTurn()
    {
        // Check to see if any AI sequences are chosen
        if (bestChoiceSequence.Count > 0)
        {
            // Get best choice, then pop from sequence
            var bestChoicePair = bestChoiceSequence[0];
            bestChoiceSequence.RemoveAt(0);

            // Make sure choice is a valid location
            if (bestChoicePair.Item2.z != -1)
            {
                // Debug
                print("AI Entity [" + selectedEntity.name + "] used [" + bestChoicePair.Item1.name + "] on location [" + bestChoicePair.Item2 + "]");

                // Select Action
                SelectAction(bestChoicePair.Item1);

                // Select Location
                yield return ConfirmLocationAI(bestChoicePair.Item2);
            }

            // Give small pause before next action
            yield return new WaitForSeconds(bufferTime);

            // Check another action
            yield return PerformEnemyTurn();
        }
        else
        {
            // End the turn
            yield return EndTurn();
        }
    }

    public void SelectAction(Action action)
    {
        // Dip if u already have location selected
        if (selectedLocation != Vector3Int.zero) return;

        // Update selected action (could be null)
        this.selectedAction = action;

        if (action != null)
        {
            // Debug
            //print("Action " + action.name + " was selected.");
        }
        else
        {
            // Debug
            //print("Action was de-selected.");
        }

        // Trigger event
        GameEvents.instance.TriggerOnActionSelect(selectedEntity, selectedAction);
    }

    public void SelectLocation(Vector3Int location)
    {
        this.selectedLocation = location;

        if (location == Vector3Int.zero)
        {
            // Debug
            // print("Location " + location + " was de-selected.");

            // Clean up selected tiles, etc
            CleanThreats(selectedAction);

            return;
        }

        // Debug
        // print("Location " + location + " was selected.");

        // Add threatened locations to table
        var locations = selectedAction.GetThreatenedLocations(selectedEntity, location);
        threatenLocationsHashtable.Add(selectedAction, locations);

        // Show threats
        foreach (var loc in locations)
        {
            // Trigger events
            GameEvents.instance.TriggerOnActionThreatenLocation(loc);
        }

        // Trigger event
        GameEvents.instance.TriggerOnLocationSelect(selectedAction, location); // Needed?
    }

    public void ConfirmAction()
    {
        // Debug
        print("Player [" + selectedEntity.name + "] used [" + selectedAction.name + "] on location [" + selectedLocation + "]");

        // Perform different logic based on action type
        switch (selectedAction.actionType)
        {
            case ActionType.Instant:

                // Trigger event
                GameEvents.instance.TriggerOnActionConfirm(selectedEntity, selectedAction, selectedLocation);

                // Perform immediately
                coroutine = StartCoroutine(PerformAction(selectedEntity, selectedAction, selectedLocation));

                // Clean up selected tiles, etc
                CleanThreats(selectedAction);

                break;
            case ActionType.Reactive:

                // Store in dictionary
                if (reactiveActionsHastable.TryGetValue(selectedLocation, out var entityActionPairList))
                {
                    // Add to existing list
                    entityActionPairList.Add((selectedEntity, selectedAction));
                }
                else
                {
                    // Add new entry
                    reactiveActionsHastable.Add(selectedLocation, new List<(Entity, Action)>() { (selectedEntity, selectedAction) });
                }



                // Immediately end turn after
                coroutine = StartCoroutine(EndTurn());

                break;
            case ActionType.Delayed:

                // Store in dictionary somewhere to perform later
                delayedActionsHashtable.Add(selectedEntity, (selectedAction, selectedLocation));

                // Immediately end turn after
                coroutine = StartCoroutine(EndTurn());

                break;
        }

        // Reset selected values
        selectedAction = null;
        selectedLocation = Vector3Int.zero;
    }

    private IEnumerator ConfirmLocationAI(Vector3Int location)
    {
        this.selectedLocation = location;

        // Debug
        // print("Location " + location + " was selected.");

        // Trigger event
        GameEvents.instance.TriggerOnActionConfirm(selectedEntity, selectedAction, location);

        // Perform action
        yield return PerformAction(selectedEntity, selectedAction, selectedLocation);

        // Reset selected values
        selectedAction = null;
        selectedLocation = Vector3Int.zero;
    }

    private IEnumerator PerformAction(Entity entity, Action action, Vector3Int location)
    {
        // Trigger event
        GameEvents.instance.TriggerOnActionPerformStart(entity, action, location, room);

        // Exhaust selected die
        action.die.Exhaust();

        // Perform the action and wait until it's finished
        yield return action.Perform(entity, location, room);

        // After the action is performed, re-enable UI
        GameEvents.instance.TriggerOnActionPerformEnd(entity, action, location, room);

        // Done
        yield return null;
    }

    private IEnumerator PerformDelayedAction(Entity entity)
    {
        // See if there is a reactive action at this location
        if (delayedActionsHashtable.TryGetValue(entity, out var actionLocationPair))
        {
            // Perform the action
            yield return PerformAction(entity, actionLocationPair.Item1, actionLocationPair.Item2);

            // Remove entry
            delayedActionsHashtable.Remove(entity);

            // Clean up selected tiles, etc
            CleanThreats(actionLocationPair.Item1);
        }
    }

    public IEnumerator PerformReactiveAction(Vector3Int location)
    {
        // See if there is a reactive action at this location
        if (reactiveActionsHastable.TryGetValue(location, out var entityActionPairList))
        {
            // Loop through each pair and activate it
            foreach (var entityActionPair in entityActionPairList)
            {
                // Perform the action
                yield return PerformAction(entityActionPair.Item1, entityActionPair.Item2, location);

                // Clean up selected tiles, etc
                CleanThreats(entityActionPair.Item2);
            }

            // Remove entry from table
            reactiveActionsHastable.Remove(location);
        }
    }

    private void CleanThreats(Action action)
    {
        // See if action exists in table
        if (threatenLocationsHashtable.TryGetValue(action, out var locations))
        {
            foreach (var location in locations)
            {
                // Trigger event
                GameEvents.instance.TriggerOnActionUnthreatenLocation(location);
            }

            // Remove entry
            threatenLocationsHashtable.Remove(action);
        }
        else
        {
            // Debug
            print("CANNOT CLEAN ACTION: " + action.name);
        }
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
            print("Starting New Round: " + (roundNumber + 1));

            // Make new round
            yield return StartRound();
        }
        else
        {
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
        GameEvents.instance.TriggerOnExitFloor(room);
    }

    public void TravelToNextFloor()
    {
        // Exit current floor
        ExitFloor();

        // We want to consider the exit we chose's number
        var roomIndex = DataManager.instance.GetNextRoomIndex();
        // Set next room based on exit's index
        DataManager.instance.SetNextRoom(roomIndex);

        // Reload this scene on player
        var location = RoomUI.instance.GetLocationCenter(room.player.location);
        TransitionManager.instance.ReloadScene(location);
    }

    public void ReturnToMainMenu()
    {
        // Exit current floor
        ExitFloor();

        // Load main menu on player
        var location = RoomUI.instance.GetLocationCenter(room.player.location);
        TransitionManager.instance.LoadMainMenuScene(location);
    }


    // TEMP CURSOR SHIT
    public void SetGrabCursor()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, cursorMode);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
