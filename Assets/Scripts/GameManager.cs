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

    [Header("Cursor")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    [Header("Entity Data")]
    [SerializeField] private Room room;
    [SerializeField] private RoomGenerator roomGenerator;
    [SerializeField] private EnemyGenerator enemyGenerator;

    [Header("Data")]
    [SerializeField] private int roundNumber = 0;

    private Queue<Entity> turnQueue;
    private Entity selectedEntity;
    private Action selectedAction;
    private Vector3Int selectedLocation;
    List<(Action, Vector3Int)> bestChoiceSequence;

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

        // Generate the room
        yield return GenerateRoom();

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
                    // Spawn a core
                    var core = enemyGenerator.GenerateCore();

                    // Populate the room
                    room.Populate(core);

                    // Generate a random enemy
                    var enemy = enemyGenerator.GenerateEnemy();

                    // Populate the room
                    room.Populate(enemy);
                }

                break;
            case -1:
                // READD THIS LATER

                // Generate a shopkeeper
                var shopkeeper = enemyGenerator.GenerateShopkeeper();

                // Populate the room
                room.Populate(shopkeeper);

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
        room.Populate(player);

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
        foreach (var enemy in room.enemies)
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

    private IEnumerator StartRound()
    {
        // Increment round
        roundNumber++;

        // Reroll all die
        // yield return ResetAllDie();

        // Generate turn order
        // Need to make queue to decide enemy turn order
        yield return GenerateTurnQueue();

        // Start turn
        yield return StartTurn();
    }

    private IEnumerator ResetAllDie()
    {
        // Loop through all entities in room
        foreach (var entity in room.GetAllEntities())
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

        yield return null;
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

        // Check if that entity has a prepared action
        if (selectedEntity.preparedAction.Item1 != null)
        {
            // Debug
            print("Forming Slow Action: " + selectedEntity.preparedAction.Item1.name);

            // Perform that action
            selectedAction = selectedEntity.preparedAction.Item1;
            selectedLocation = selectedEntity.preparedAction.Item2;
            yield return PerformSelectedAction();

            // Reset prepared action
            selectedEntity.preparedAction = (null, Vector3Int.back);
        }

        // Reset actions now
        ResetActions(selectedEntity);

        // Check if the enity has an ai, if so, let them decide their action
        if (selectedEntity.AI != null)
        {
            // Get best sequence of actions
            bestChoiceSequence = selectedEntity.AI.GenerateNewBestDecision(selectedEntity, room, room.player);

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
                print("Entity: " + selectedEntity.name + " used: " + bestChoicePair.Item1.name + " on location: " + bestChoicePair.Item2);

                // Select Action
                SelectAction(bestChoicePair.Item1);

                // Select Location
                yield return ConfirmLocationAI(bestChoicePair.Item2);
            }

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
            print("Action " + action.name + " was selected.");
        }
        else
        {
            // Debug
            print("Action was de-selected.");
        }

        // Trigger event
        GameEvents.instance.TriggerOnActionSelect(selectedEntity, selectedAction);
    }

    public void ConfirmLocation(Vector3Int location)
    {
        this.selectedLocation = location;

        // Debug
        print("Location " + location + " was selected.");

        // Trigger event
        GameEvents.instance.TriggerOnLocationSelect(selectedEntity, selectedAction, location);

        // Check action type
        if (selectedAction.actionSpeed is ActionSpeed.Slow)
        {
            // Debug
            print("Preparing Slow Action: " + selectedAction.name);

            // Store action
            selectedEntity.preparedAction = (selectedAction, selectedLocation);

            // Immediately end turn
            coroutine = StartCoroutine(EndTurn());
        }
        else
        {
            // Perform the action right away

            // Start routine
            coroutine = StartCoroutine(PerformSelectedAction());
        }

    }

    private IEnumerator ConfirmLocationAI(Vector3Int location)
    {
        this.selectedLocation = location;

        // Debug
        print("Location " + location + " was selected.");

        // Exhaust selected die
        // selectedAction.die.Exhaust();

        // Trigger event
        // GameEvents.instance.TriggerOnDieExhaust(selectedAction.die);

        // Trigger event
        GameEvents.instance.TriggerOnLocationSelect(selectedEntity, selectedAction, location);

        // Perform action
        yield return PerformSelectedAction();
    }

    private IEnumerator PerformSelectedAction()
    {
        // Trigger event
        GameEvents.instance.TriggerOnActionPerformStart(selectedEntity, selectedAction, selectedLocation, room);

        // Exhaust selected die
        selectedAction.die.Exhaust();

        // Perform the action and wait until it's finished
        yield return selectedAction.Perform(selectedEntity, selectedLocation, room);

        // After the action is performed, re-enable UI
        GameEvents.instance.TriggerOnActionPerformEnd(selectedEntity, selectedAction, selectedLocation, room);

        // Reset selected values
        selectedAction = null;
        selectedLocation = Vector3Int.zero;

        // Done
        yield return null;
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
        var roomIndex = room.roomExit.destinationIndex;
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
