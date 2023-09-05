using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Cursor")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    [Header("Entity Data")]
    [SerializeField] private Room room;
    [SerializeField] private RoomGenerator roomGenerator;
    [SerializeField] private EnemyGenerator enemyGenerator;

    [Header("Data")]
    [SerializeField] private int roundNumber = 0;
    [SerializeField] public GameSettings gameSettings;
    [SerializeField] public bool gameOver;

    private Queue<Entity> turnQueue;
    [SerializeField] private Entity selectedEntity;
    [SerializeField] private Action selectedAction;
    [SerializeField] private Vector3Int selectedLocation;
    [SerializeField] private List<Vector3Int> selectedThreats;
    private List<(Action, Vector3Int)> bestChoiceSequence;
    private Dictionary<(Entity, Action), List<Vector3Int>> reactiveActionsHastable;
    private Dictionary<(Entity, Action), List<Vector3Int>> delayedActionsHashtable;

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

        // Initialize lists
        bestChoiceSequence = new List<(Action, Vector3Int)>();
        selectedThreats = new List<Vector3Int>();
        reactiveActionsHastable = new Dictionary<(Entity, Action), List<Vector3Int>>();
        delayedActionsHashtable = new Dictionary<(Entity, Action), List<Vector3Int>>();

        gameOver = false;
    }

    private void Start()
    {
        // Start the game
        coroutine = StartCoroutine(EnterFloor());
    }

    private void Update()
    {
        // If right click anywhere
        if (Input.GetMouseButtonDown(1) && selectedEntity is Player)
        {
            print("hit");
            // Then cancel it
            SelectAction(null);
        }
    }

    private IEnumerator EnterFloor()
    {
        // Reset round count
        roundNumber = 1;

        // Generate the room
        yield return GenerateFloor();

        // Trigger event
        GameEvents.instance.TriggerGenerateFloor(room);

        // Generate player
        yield return GeneratePlayer();

        // Trigger event
        GameEvents.instance.TriggerOnEnterFloor(room.player);

        // Generate enemies
        yield return GenerateEnemies();

        // Generate barrels
        yield return GenerateBarrels();

        // Generate keys
        yield return GenerateKeys();

        // Generate gold
        yield return GenerateGold();

        // Open scene on player
        var location = RoomUI.instance.GetLocationCenter(room.player.location);
        TransitionManager.instance.OpenScene(location);

        // Wait a bit before starting game
        yield return new WaitForSeconds(gameSettings.gameStartBufferTime);

        // Start the first round
        yield return StartRound();
    }

    private IEnumerator GenerateFloor()
    {
        switch (DataManager.instance.GetCurrentRoom())
        {
            case RoomType.Normal:

                if (DataManager.instance.GetRoomNumber() < 3)
                {
                    room = roomGenerator.GenerateRoom(RoomSize.Small);
                }
                else
                {
                    room = roomGenerator.GenerateRoom(RoomSize.Medium);
                }

                break;
            case RoomType.Shop:

                // Generate a shop
                room = roomGenerator.GenerateShop();

                break;
            case RoomType.Arena:

                // Generate an arena
                room = roomGenerator.GenerateArena();

                break;
            default:
                throw new System.Exception("ERROR CREATING ROOM WITH ROOM INDEX");
        }

        yield return null;
    }

    private IEnumerator GeneratePlayer()
    {
        // Get player from data
        var player = DataManager.instance.GetPlayer();

        // Populate the room
        room.SpawnPlayer(player);

        // Finish
        yield return null;
    }

    private IEnumerator GenerateEnemies()
    {
        switch (DataManager.instance.GetCurrentRoom())
        {
            case RoomType.Normal:

                // Spawn enemies
                int num = Mathf.Min(DataManager.instance.GetRoomNumber(), 3);
                for (int i = 0; i < num; i++)
                {
                    // Generate a random enemy
                    var enemy = enemyGenerator.GenerateEnemy(i);

                    // Populate the room
                    room.SpawnEntity(enemy);
                }

                break;
            case RoomType.Shop:

                // Generate a shopkeeper
                var shopkeeper = enemyGenerator.GenerateShopkeeper();

                // Populate the room
                room.SpawnEntity(shopkeeper);

                // Open Shop if shop floor
                CheckShop(shopkeeper.inventory);

                break;
            case RoomType.Arena:

                // Generate a boss
                var boss = enemyGenerator.GenerateBoss();

                // Populate the room
                room.SpawnEntity(boss, true);

                // Generate a random enemy
                var enem = enemyGenerator.GenerateEnemy();

                // Populate the room
                room.SpawnEntity(enem);

                break;

            default:
                throw new System.Exception("ERROR CREATING ENEMIES WITH ROOM INDEX: ");
        }

        // Finish
        yield return null;
    }

    private IEnumerator GenerateBarrels()
    {
        switch (DataManager.instance.GetCurrentRoom())
        {
            case RoomType.Normal:

                // Create keys based on room number
                int num = Mathf.Min(DataManager.instance.GetRoomNumber(), 2);
                for (int i = 0; i < num; i++)
                {
                    // Spawn a barrel
                    room.SpawnEntity(enemyGenerator.barrel.Copy());
                }

                break;
        }

        // Finish
        yield return null;
    }

    private IEnumerator GenerateKeys()
    {
        switch (DataManager.instance.GetCurrentRoom())
        {
            case RoomType.Normal:

                // Create keys based on room number
                int num = Mathf.Min(DataManager.instance.GetRoomNumber(), 2);
                for (int i = 0; i < num; i++)
                {
                    // Spawn a key
                    room.SpawnPickup(PickUpType.Key);
                }

                break;
        }

        // Finish
        yield return null;
    }

    private IEnumerator GenerateGold()
    {
        switch (DataManager.instance.GetCurrentRoom())
        {
            case RoomType.Normal:

                // Create keys based on room number
                int num = Mathf.Min(DataManager.instance.GetRoomNumber(), 2);
                for (int i = 0; i < num; i++)
                {
                    // Spawn a key
                    room.SpawnPickup(PickUpType.Gold);
                }

                break;
        }

        // Finish
        yield return null;
    }

    private IEnumerator GenerateTurnQueue()
    {
        // Initialize
        turnQueue = new Queue<Entity>();

        // First go enemies
        foreach (var entity in room.hostileEntities)
        {
            turnQueue.Enqueue(entity);
        }

        // Next go neutrals
        foreach (var entity in room.neutralEntities)
        {
            turnQueue.Enqueue(entity);
        }

        // Next go allies
        foreach (var entity in room.alliedEntities)
        {
            turnQueue.Enqueue(entity);
        }

        // Player goes last player
        turnQueue.Enqueue(room.player);

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

        // Check if game is over
        if (!gameOver)
        {
            // Generate turn order
            yield return GenerateTurnQueue();

            // Start turn
            yield return StartTurn();
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

        // Perform any delayed actions stored by this entity
        yield return PerformDelayedAction(selectedEntity);

        // Perform any reactive actions stored by this entity
        yield return PerformAnyReactiveActions(selectedEntity);

        // Debug
        print("Turn Start: " + selectedEntity.name);

        // Trigger event 
        GameEvents.instance.TriggerOnTurnStart(selectedEntity);

        // Clear any reactive
        ClearReativeActions(selectedEntity);

        // Reset actions now
        yield return ResetActions(selectedEntity);

        // Check if the enity has an ai, if so, let them decide their action
        if (selectedEntity.AI != null)
        {
            // Get best sequence of actions
            bestChoiceSequence = selectedEntity.AI.GenerateSequenceOfActions(selectedEntity, room, room.player);

            // Then start performing those actions
            yield return PerformTurnAI();
        }
    }

    public void SelectAction(Action action)
    {
        // if (selectedThreats.Count > 0) return;

        // If you already have a selected location
        if (selectedLocation != Vector3Int.zero)
        {
            // Unselect it
            SelectLocation(Vector3Int.zero);
        }

        // Check all 4 cases
        if (selectedAction == null && action == null)
        {
            // Do nothing.
        }
        else if (selectedAction != null && action == null)
        {
            print("De-select current action.");

            selectedAction = null;
            // Trigger event
            GameEvents.instance.TriggerOnActionSelect(selectedEntity, null);
        }
        else if (selectedAction == null && action != null)
        {
            print("Select new action.");

            selectedAction = action;
            // Trigger event
            GameEvents.instance.TriggerOnActionSelect(selectedEntity, action);
        }
        else if (selectedAction != null && action != null)
        {
            if (selectedAction == action)
            {
                print("De-select current action.");

                selectedAction = null;
                // Trigger event
                GameEvents.instance.TriggerOnActionSelect(selectedEntity, null);
            }
            else
            {
                print("Swap selected actions.");

                selectedAction = action;
                // Trigger events
                GameEvents.instance.TriggerOnActionSelect(selectedEntity, null);
                GameEvents.instance.TriggerOnActionSelect(selectedEntity, action);
            }
        }
        else throw new System.Exception("UNHANDLED ACTION SELECT CASE ENCOUNTER!");
    }

    public void SelectLocation(Vector3Int location)
    {
        if (location == Vector3Int.zero)
        {
            if (selectedThreats.Count > 0)
            {
                // Clean up selected tiles, etc
                // Trigger event
                GameEvents.instance.TriggerOnActionUnthreatenLocation(selectedAction, selectedThreats);

                // Remove threats
                selectedThreats.Clear();
            }

            if (selectedLocation != Vector3Int.zero && selectedAction.actionType == ActionType.Attack)
            {
                // Sheathe weapon
                GameEvents.instance.TriggerOnEntitySheatheWeapon(selectedEntity, selectedAction.weapon);
            }
        }
        else
        {
            // Add threatened locations to table
            selectedThreats = selectedAction.GetThreatenedLocations(selectedEntity, location);

            // Show threats
            GameEvents.instance.TriggerOnActionThreatenLocation(selectedAction, selectedThreats);

            if (selectedLocation == Vector3Int.zero && selectedAction.actionType == ActionType.Attack)
            {
                // Calculate direction
                Vector3Int direction = location - selectedEntity.location;
                direction.Clamp(-Vector3Int.one, Vector3Int.one);

                // Draw weapon
                GameEvents.instance.TriggerOnEntityDrawWeapon(selectedEntity, direction, selectedAction.weapon);
            }
        }

        // Update location
        this.selectedLocation = location;

        // Trigger event
        GameEvents.instance.TriggerOnLocationSelect(selectedEntity, selectedAction, selectedLocation);
    }

    public void ConfirmAction()
    {
        // Debug
        print("Player [" + selectedEntity.name + "] used [" + selectedAction.name + "] on location [" + selectedLocation + "]");

        // Trigger event
        GameEvents.instance.TriggerOnActionConfirm(selectedEntity, selectedAction, selectedLocation);

        // Perform different logic based on action type
        switch (selectedAction.actionSpeed)
        {
            case ActionSpeed.Instant:

                // Perform immediately
                coroutine = StartCoroutine(PerformAction(selectedEntity, selectedAction, selectedLocation, selectedThreats));

                break;
            case ActionSpeed.Reactive:

                // Save action pair to table
                reactiveActionsHastable[(selectedEntity, selectedAction)] = new List<Vector3Int>(selectedThreats);

                // Immediately end turn after
                coroutine = StartCoroutine(EndTurn());

                break;
            case ActionSpeed.Delayed:

                // Save action pair to table
                delayedActionsHashtable[(selectedEntity, selectedAction)] = new List<Vector3Int>(selectedThreats);

                // Immediately end turn after
                coroutine = StartCoroutine(EndTurn());

                break;
        }
    }

    private IEnumerator PerformTurnAI()
    {
        // Check to see if any AI sequences are chosen
        if (bestChoiceSequence.Count > 0)
        {
            // Get best choice, then pop from sequence
            var bestChoicePair = bestChoiceSequence[0];
            bestChoiceSequence.RemoveAt(0);

            // Wait a bit before performing action
            yield return new WaitForSeconds(gameSettings.aiBufferTime);

            // Make sure choice is a valid location
            if (bestChoicePair.Item2.z != -1)
            {
                // Select Action
                SelectAction(bestChoicePair.Item1);

                // Select Location
                SelectLocation(bestChoicePair.Item2);

                // Confirm Action
                yield return ConfirmActionAI();
            }

            // Reset values
            selectedAction = null;
            selectedLocation = Vector3Int.zero;
            selectedThreats.Clear();

            // Check another action
            yield return PerformTurnAI();
        }
        else
        {
            // End the turn
            yield return EndTurn();
        }
    }

    private IEnumerator ConfirmActionAI()
    {
        // Debug
        print("AI [" + selectedEntity.name + "] used [" + selectedAction.name + "] on location [" + selectedLocation + "]");

        // Trigger event
        GameEvents.instance.TriggerOnActionConfirm(selectedEntity, selectedAction, selectedLocation);

        // Perform different logic based on action type
        switch (selectedAction.actionSpeed)
        {
            case ActionSpeed.Instant:

                // Perform immediately
                yield return PerformAction(selectedEntity, selectedAction, selectedLocation, selectedThreats);

                break;
            case ActionSpeed.Reactive:

                // Save action pair to table
                reactiveActionsHastable[(selectedEntity, selectedAction)] = new List<Vector3Int>(selectedThreats);

                break;
            case ActionSpeed.Delayed:

                // Save action pair to table
                delayedActionsHashtable[(selectedEntity, selectedAction)] = new List<Vector3Int>(selectedThreats);

                break;
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
        // Reset selected values
        selectedAction = null;
        selectedLocation = Vector3Int.zero;
        selectedThreats.Clear();

        // Exhaust all die
        foreach (var action in selectedEntity.GetActions())
        {
            action.die.Exhaust();
        }

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
    }

    public void TravelToNextFloor()
    {
        // Exit current floor
        ExitFloor();

        // Set next room based on exit's index
        DataManager.instance.SetNextRoom();

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

    public void Restart()
    {
        // Exit current floor
        ExitFloor();

        DataManager.instance.CreateNewPlayer();

        // Reload this scene with new player
        TransitionManager.instance.ReloadScene(Vector3.zero);
    }

    private IEnumerator ResetActions(Entity entity)
    {
        // Player should have buffer time
        if (entity is Player)
        {
            // Visually loop die
            foreach (var action in entity.GetActions())
            {
                GameEvents.instance.TriggerOnDieLoop(action.die);
            }

            // Buffer
            yield return new WaitForSeconds(gameSettings.diceRollTime);

            // Set new values
            foreach (var action in entity.GetActions())
            {
                // Roll die with event
                action.die.Roll();

                // Replenish die with event
                action.die.Replenish();

                // Buffer
                yield return new WaitForSeconds(0.25f);
            }

            // Buffer
            yield return new WaitForSeconds(0.25f);
        }
        else
        {
            // Set new values
            foreach (var action in entity.GetActions())
            {
                // Replenish die with event
                action.die.Replenish();

                // Roll die with event
                action.die.Roll();
            }
        }

    }

    private IEnumerator PerformAction(Entity entity, Action action, Vector3Int location, List<Vector3Int> threatenedLocations)
    {
        // Trigger event
        GameEvents.instance.TriggerOnActionPerformStart(entity, action, location, room);

        // Exhaust selected die
        action.die.Exhaust();

        // Perform the action and wait until it's finished
        yield return action.Perform(entity, location, threatenedLocations, room);

        // After the action is performed, re-enable UI
        GameEvents.instance.TriggerOnActionPerformEnd(entity, action, location, room);

        // Clean up selected tiles, etc
        GameEvents.instance.TriggerOnActionUnthreatenLocation(action, threatenedLocations);

        if (action.actionType == ActionType.Attack)
        {
            // Sheathe weapon
            GameEvents.instance.TriggerOnEntitySheatheWeapon(entity, action.weapon);
        }

        // Reset selected values
        selectedAction = null;
        selectedLocation = Vector3Int.zero;
        selectedThreats.Clear();
    }

    private IEnumerator PerformDelayedAction(Entity entity)
    {
        // Loop through each pair
        bool done = false;
        foreach (var entityActionPair in delayedActionsHashtable)
        {
            var action = entityActionPair.Key.Item2;
            var targets = entityActionPair.Value;

            // Check for any matches
            if (entityActionPair.Key.Item1 == entity)
            {
                foreach (var location in targets)
                {
                    if (room.GetEntityAtLocation(location) is Player)
                    {
                        // Perform the action
                        yield return PerformAction(entity, action, targets[0], targets);

                        break;
                    }
                }

                // Remove entry
                delayedActionsHashtable.Remove(entityActionPair.Key);

                // Hide threats
                GameEvents.instance.TriggerOnActionUnthreatenLocation(action, targets);

                // Sheathe weapon
                if (action.actionType == ActionType.Attack)
                    GameEvents.instance.TriggerOnEntitySheatheWeapon(entity, action.weapon);

                done = true;
            }

            if (done) break;
        }
    }

    private IEnumerator PerformAnyReactiveActions(Entity entity)
    {
        bool done = false;
        foreach (var entityActionPair in reactiveActionsHastable)
        {
            // Check if this entity has any reactive actions
            if (entityActionPair.Key.Item1 == entity)
            {
                // Check if the locations contain the player
                foreach (var location in entityActionPair.Value)
                {
                    if (room.GetEntityAtLocation(location) is Player)
                    {
                        // Parse data
                        var action = entityActionPair.Key.Item2;
                        var targets = entityActionPair.Value;

                        // Perform the action
                        yield return PerformAction(entity, action, targets[0], targets);

                        // Remove entry
                        reactiveActionsHastable.Remove(entityActionPair.Key);

                        // Stop
                        done = true;
                        break;
                    }
                }
            }

            if (done) break;
        }
    }

    public IEnumerator PerformReactiveAction(Vector3Int location)
    {
        // Loop through each pair
        foreach (var entityActionPair in reactiveActionsHastable)
        {
            // Check if action threatens this location
            if (entityActionPair.Value.Contains(location))
            {
                // Parse data
                var entity = entityActionPair.Key.Item1;
                var action = entityActionPair.Key.Item2;
                var targets = entityActionPair.Value;

                // Perform the action
                yield return PerformAction(entity, action, targets[0], targets);

                // Remove entry
                reactiveActionsHastable.Remove(entityActionPair.Key);

                // Stop
                break;
            }
        }
    }

    public void ClearReativeActions(Entity entity)
    {
        // Loop through each pair
        foreach (var entityActionPair in reactiveActionsHastable)
        {
            // Check if action threatens this location
            if (entityActionPair.Key.Item1 == entity)
            {
                // Parse data
                var action = entityActionPair.Key.Item2;
                var targets = entityActionPair.Value;

                // Remove entry
                reactiveActionsHastable.Remove(entityActionPair.Key);

                // Hide threats
                GameEvents.instance.TriggerOnActionUnthreatenLocation(action, targets);

                // Sheathe weapon
                if (action.actionType == ActionType.Attack)
                    GameEvents.instance.TriggerOnEntitySheatheWeapon(entity, action.weapon);

                // Stop
                break;
            }
        }
    }

    public void ClearDelayedActions(Entity entity)
    {
        // Loop through each pair
        foreach (var entityActionPair in delayedActionsHashtable)
        {
            // Check if action threatens this location
            if (entityActionPair.Key.Item1 == entity)
            {
                // Parse data
                var action = entityActionPair.Key.Item2;
                var targets = entityActionPair.Value;

                // Remove entry
                delayedActionsHashtable.Remove(entityActionPair.Key);

                // Hide threats
                GameEvents.instance.TriggerOnActionUnthreatenLocation(action, targets);

                // Sheathe weapon
                if (action.actionType == ActionType.Attack)
                    GameEvents.instance.TriggerOnEntitySheatheWeapon(entity, action.weapon);

                // Stop
                break;
            }
        }
    }

    public void InspectLocation(Vector3Int location)
    {
        // Get entity at the location
        Entity entity = room.GetEntityAtLocation(location);

        if (entity != null)
        {
            List<Vector3Int> locations = null;

            if (entity != null)
            {
                // Loop through each pair
                foreach (var entityActionPair in delayedActionsHashtable)
                {
                    // Check if action belongs to the entity
                    if (entityActionPair.Key.Item1 == entity)
                    {
                        // Update targets
                        locations = entityActionPair.Value;

                        break;
                    }
                }
            }

            // Trigger event
            GameEvents.instance.TriggerOnEntityInspect(entity, locations);
        }

    }

    // TEMP SHOP LOGIC
    private void CheckShop(Inventory inventory)
    {
        // If we are currently on a shop floor
        if (DataManager.instance.GetCurrentRoom() == RoomType.Shop)
        {
            GameEvents.instance.TriggerOnOpenShop(room.player, inventory);
        }
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
