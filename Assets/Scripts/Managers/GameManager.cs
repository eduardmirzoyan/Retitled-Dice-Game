using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Entity Data")]
    [SerializeField] private Room room;
    [SerializeField] private RoomGenerator roomGenerator;
    [SerializeField] private EnemyGenerator enemyGenerator;
    [SerializeField] private EntityEnchantmentGenerator enchantmentGenerator;

    [Header("Data")]
    [SerializeField] private int roundNumber = 0;
    [SerializeField] public GameSettings gameSettings;
    [SerializeField] public bool gameOver;

    [Header("Debug")]
    [SerializeField] private List<Entity> turnQueue;
    [SerializeField] private Entity selectedEntity;
    [SerializeField] private Action selectedAction;
    [SerializeField] private Vector3Int selectedLocation;
    [SerializeField] private List<Vector3Int> selectedThreats;

    [Header("Logging")]
    [SerializeField] private bool logGameStates;
    [SerializeField] private bool logEntityActions;

    private List<(Action, Vector3Int)> bestChoiceSequence;
    private Dictionary<(Entity, Action), List<Vector3Int>> delayedActionsHashtable;

    private Coroutine coroutine;
    public static GameManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Initialize lists
        bestChoiceSequence = new List<(Action, Vector3Int)>();
        selectedThreats = new List<Vector3Int>();
        delayedActionsHashtable = new Dictionary<(Entity, Action), List<Vector3Int>>();

        // Initialize values
        selectedEntity = null;
        selectedAction = null;
        selectedLocation = Vector3Int.back;

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
            // Then cancel it
            SelectAction(null);
        }

        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.L))
        {
            var enchantments = enchantmentGenerator.GenerateEnchantmentSet();
            GameEvents.instance.TriggerOnPresentEnchantments(enchantments);
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
        var location = RoomManager.instance.GetLocationCenter(room.player.location);
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
            case RoomType.Boss:

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
                int num = DataManager.instance.GetRoomNumber();
                if (num < 3)
                {
                    for (int i = 0; i < num; i++)
                    {
                        // Generate up to first 2 enemies
                        var enemy = enemyGenerator.GenerateEnemy(i);

                        // Populate the room
                        room.SpawnEntity(enemy);
                    }
                }
                else
                {
                    // Generate 3 random
                    for (int i = 0; i < 3; i++)
                    {
                        // Generate a random enemy
                        var enemy = enemyGenerator.GenerateEnemy();

                        // Populate the room
                        room.SpawnEntity(enemy);
                    }
                }

                break;
            case RoomType.Shop:

                // Generate a shopkeeper in hardcoded location
                var shopkeeper = enemyGenerator.GenerateShopkeeper();
                Vector3Int location = new(12, 13);

                // Populate the room
                room.SpawnEntity(shopkeeper, location);

                // Generate a blacksmith
                var blacksmith = enemyGenerator.GenerateBlacksmith();
                location = new(13, 13);
                room.SpawnEntity(blacksmith, location);

                break;
            case RoomType.Boss:

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
        turnQueue = new List<Entity>();

        // First go enemies
        foreach (var entity in room.hostileEntities)
        {
            turnQueue.Add(entity);
        }

        // Next go neutrals
        foreach (var entity in room.neutralEntities)
        {
            turnQueue.Add(entity);
        }

        // Next go allies
        foreach (var entity in room.alliedEntities)
        {
            turnQueue.Add(entity);
        }

        // Player goes last
        turnQueue.Add(room.player);

        // Debug
        if (logGameStates)
        {
            string result = "Generated Queue: ";
            foreach (var ent in turnQueue.ToArray())
            {
                result += ent.name + " -> ";
            }
            result += "END";
            print(result);
        }

        // Finish
        yield return null;
    }

    private IEnumerator StartRound()
    {
        // Increment round
        roundNumber++;

        // Debug
        if (logGameStates) print("Starting Round: " + roundNumber);

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
        // Pop first entity from queue
        selectedEntity = turnQueue[0];
        turnQueue.RemoveAt(0);

        // If the entity is dead, skip
        if (selectedEntity.currentHealth == 0)
        {
            if (logEntityActions) print("Entity: " + selectedEntity.name + " is dead so skipping turn.");

            // End turn
            yield return EndTurn();
        }

        // Perform any delayed actions stored by this entity
        yield return PerformDelayedAction(selectedEntity);

        // Debug
        if (logGameStates) print("Turn Start: " + selectedEntity.name);

        // Trigger event 
        GameEvents.instance.TriggerOnTurnStart(selectedEntity);

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
        if (selectedLocation != Vector3Int.back)
        {
            // Unselect it
            SelectLocation(Vector3Int.back);
        }

        // Check all 4 cases
        if (selectedAction == null && action == null)
        {
            // Do nothing.
        }
        else if (selectedAction != null && action == null)
        {
            //print("De-select current action.");
            selectedAction = null;

            // Trigger event
            GameEvents.instance.TriggerOnActionSelect(selectedEntity, null);
        }
        else if (selectedAction == null && action != null)
        {
            //print("Select new action.");
            if (action.die.isExhausted) return;

            selectedAction = action;
            // Trigger event
            GameEvents.instance.TriggerOnActionSelect(selectedEntity, action);
        }
        else if (selectedAction != null && action != null)
        {
            if (action.die.isExhausted) return;

            if (selectedAction == action)
            {
                //print("De-select current action.");

                selectedAction = null;
                // Trigger event
                GameEvents.instance.TriggerOnActionSelect(selectedEntity, null);
            }
            else
            {
                //print("Swap selected actions.");

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
        if (selectedLocation == Vector3Int.back && location == Vector3Int.back)
        {
            // Do nothing.
        }
        else if (selectedLocation != Vector3Int.back && location == Vector3Int.back)
        {
            // print("De-select current location.");

            selectedLocation = Vector3Int.back;

            // Trigger event
            GameEvents.instance.TriggerOnLocationSelect(selectedEntity, selectedAction, Vector3Int.back);

            // Hide threats
            GameEvents.instance.TriggerOnActionUnthreatenLocation(selectedAction, selectedThreats);

            // Clear
            selectedThreats.Clear();

            if (selectedAction.actionType == ActionType.Attack)
            {
                // Sheathe weapon
                GameEvents.instance.TriggerOnEntitySheatheWeapon(selectedEntity, selectedAction.weapon);
            }
        }
        else if (selectedLocation == Vector3Int.back && location != Vector3Int.back)
        {
            // print("Select new location.");

            selectedLocation = location;

            // Trigger event
            GameEvents.instance.TriggerOnLocationSelect(selectedEntity, selectedAction, location);

            if (selectedAction.actionType == ActionType.Attack)
            {
                // Calculate direction
                Vector3Int direction = location - selectedEntity.location;
                direction.Clamp(-Vector3Int.one, Vector3Int.one);

                // Draw weapon
                GameEvents.instance.TriggerOnEntityDrawWeapon(selectedEntity, direction, selectedAction.weapon);
            }

            // Add threatened locations to table
            selectedThreats = selectedAction.GetThreatenedLocations(selectedEntity, location);

            // Show threats
            GameEvents.instance.TriggerOnActionThreatenLocation(selectedAction, selectedThreats);

            if (selectedAction.actionType == ActionType.Attack)
            {
                // Calculate direction
                Vector3Int direction = location - selectedEntity.location;
                direction.Clamp(-Vector3Int.one, Vector3Int.one);

                // Draw weapon
                GameEvents.instance.TriggerOnEntityDrawWeapon(selectedEntity, direction, selectedAction.weapon);
            }
        }
        else if (selectedLocation != Vector3Int.back && location != Vector3Int.back)
        {
            // If same location was selected
            if (selectedLocation == location)
            {
                // print("Same location, so we toggle off.");

                selectedLocation = Vector3Int.back;

                // Trigger event
                GameEvents.instance.TriggerOnLocationSelect(selectedEntity, selectedAction, Vector3Int.back);

                // Hide threats
                GameEvents.instance.TriggerOnActionUnthreatenLocation(selectedAction, selectedThreats);

                // Clear
                selectedThreats.Clear();

                if (selectedAction.actionType == ActionType.Attack)
                {
                    // Sheathe weapon
                    GameEvents.instance.TriggerOnEntitySheatheWeapon(selectedEntity, selectedAction.weapon);
                }
            }
            else
            {
                // print("Swap locations.");

                selectedLocation = location;

                // Trigger events
                GameEvents.instance.TriggerOnLocationSelect(selectedEntity, selectedAction, Vector3Int.back);
                GameEvents.instance.TriggerOnLocationSelect(selectedEntity, selectedAction, location);

                // Hide threats
                GameEvents.instance.TriggerOnActionUnthreatenLocation(selectedAction, selectedThreats);

                if (selectedAction.actionType == ActionType.Attack)
                {
                    // Sheathe weapon
                    GameEvents.instance.TriggerOnEntitySheatheWeapon(selectedEntity, selectedAction.weapon);
                }

                // Save new threatened locations 
                selectedThreats = selectedAction.GetThreatenedLocations(selectedEntity, location);

                // Show threats
                GameEvents.instance.TriggerOnActionThreatenLocation(selectedAction, selectedThreats);

                if (selectedAction.actionType == ActionType.Attack)
                {
                    // Calculate direction
                    Vector3Int direction = location - selectedEntity.location;
                    direction.Clamp(-Vector3Int.one, Vector3Int.one);

                    // Draw weapon
                    GameEvents.instance.TriggerOnEntityDrawWeapon(selectedEntity, direction, selectedAction.weapon);
                }
            }
        }
        else throw new System.Exception("UNHANDLED LOCATION SELECT CASE ENCOUNTER!");
    }

    public void ConfirmAction()
    {
        // Debug
        if (logEntityActions) print("Player [" + selectedEntity.name + "] used [" + selectedAction.name + "] on location [" + selectedLocation + "]");

        // Trigger event
        GameEvents.instance.TriggerOnActionConfirm(selectedEntity, selectedAction, selectedLocation);

        // Perform different logic based on action type
        switch (selectedAction.actionSpeed)
        {
            case ActionSpeed.Instant:

                // Perform immediately
                coroutine = StartCoroutine(PerformAction(selectedEntity, selectedAction, selectedLocation, selectedThreats));

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
            if (bestChoicePair.Item2 != Vector3Int.back)
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
            selectedLocation = Vector3Int.back;
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
        if (logEntityActions) print("AI [" + selectedEntity.name + "] used [" + selectedAction.name + "] on location [" + selectedLocation + "]");

        // Trigger event
        GameEvents.instance.TriggerOnActionConfirm(selectedEntity, selectedAction, selectedLocation);

        // Perform different logic based on action type
        switch (selectedAction.actionSpeed)
        {
            case ActionSpeed.Instant:

                // Perform immediately
                yield return PerformAction(selectedEntity, selectedAction, selectedLocation, selectedThreats);

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
        selectedLocation = Vector3Int.back;
        selectedThreats.Clear();

        // Exhaust all die
        foreach (var action in selectedEntity.AllActions())
        {
            action.die.Exhaust();
        }

        // Debug
        if (logGameStates) print("Turn End: " + selectedEntity.name);

        // Trigger event
        GameEvents.instance.TriggerOnTurnEnd(selectedEntity);

        // Check if queue is empty
        if (turnQueue.Count == 0)
        {
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
        var location = RoomManager.instance.GetLocationCenter(room.player.location);
        TransitionManager.instance.ReloadScene(location);
    }

    public void ReturnToMainMenu()
    {
        // Exit current floor
        ExitFloor();

        // Load main menu on player
        var location = RoomManager.instance.GetLocationCenter(room.player.location);
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
            // Buffer
            yield return new WaitForSeconds(gameSettings.diceRollTime);

            // Set new values
            foreach (var action in entity.AllActions())
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
            foreach (var action in entity.AllActions())
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
        selectedLocation = Vector3Int.back;
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

    public void EntityUseConsumble(Entity entity, Consumable consumable)
    {
        if (logEntityActions) print($"{entity.name} used {consumable.name}");

        // Start routine
        StartCoroutine(consumable.Use(entity));

        // Play sfx
        AudioManager.instance.PlaySFX("use_consumable");
    }

    public void PlayerGainEnchantment(EntityEnchantment entityEnchantment)
    {
        room.player.GainEnchantment(entityEnchantment);
    }

    public void InspectLocation(Vector3Int location)
    {
        // Get entity at the location
        Entity entity = room.GetEntityAtLocation(location);

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
