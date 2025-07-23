using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // Make sure this is included for Stack and Dictionary

public class GameManager : MonoBehaviour

{
    private Scene _previousActiveScene;

    public List<string> allRoomScenes = new List<string>();
    private List<string> availableRoomScenes = new List<string>();
    private string _nextPlayerSpawnPointName;
    public GameObject gameOverPanel;
    public string _lastEnteredDoorName;
    public static GameManager Instance;

    private Stack<string> _roomHistoryStack = new Stack<string>();
    private string _currentRoomSceneName;     // Stores the name of the room the player is currently in


    // Key: Room Scene Name (string), Value: Is Cleared (bool)
    private Dictionary<string, bool> _roomClearedStates = new Dictionary<string, bool>();

    // This will track if the player is currently backtracking (used internally by GameManager)
    private bool _isBacktracking = false; // FIXED: Consistent lowercase 't'

    void Awake()
    {
        Debug.Log("GameManager: Awake called. Instance ID: " + GetInstanceID());

        // Check if an instance already exists and handle potential duplicates
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager: Instance set and DontDestroyOnLoad called. Instance ID: " + GetInstanceID());

            SceneManager.sceneLoaded += OnSceneLoaded; // Ensure this is only called once
        }
        else
        {

            SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe any potentially rogue subscriptions
            Debug.LogWarning("GameManager: Duplicate instance found. Destroying self (GameManager): " + gameObject.name + ", Instance ID: " + GetInstanceID());
            Destroy(gameObject);
            return;
        }

        InitializeAvailableRooms();
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log("GameManager: OnDestroy called. Unsubscribed from sceneLoaded."); // ADDED: Debug log
        }
    }

    void Update()
    {
        // Your Update logic here, if any
    }

    public void GameOver()
    {
        if (gameOverPanel != null) // ADDED: Null check
        {
            gameOverPanel.SetActive(true); // Show the game over panel
        }
        Time.timeScale = 0f; // Pause game
        Debug.Log("Game Over! Time scaled to 0.");
    }


    // Call this method when a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        SceneManager.SetActiveScene(scene);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (_previousActiveScene.IsValid() && _previousActiveScene != scene)
        {
            Debug.Log("The previous scene is being unloaded");
            Debug.Log($"<color=orange>GameManager: PRE-DEACTIVATION CHECK for CURRENT scene: '{scene.name}'</color>");
            
            foreach (GameObject rootObj in _previousActiveScene.GetRootGameObjects())
            // unloads gameobjects in previous scenes for better perfomance
            {

                if (rootObj != this.gameObject && (rootObj.CompareTag("Player") == false || rootObj.GetComponent<PlayerPersist>() == null))
                {
                    rootObj.SetActive(false);

                }//makes sure gamemanager isnt removed
            }
        }

        Debug.Log($"<color=magenta>GameManager: Checking initial activity of root objects in CURRENT scene: '{scene.name}'</color>");
            Debug.Log($"<color=magenta>GameManager: POST-DEACTIVATION CHECK for CURRENT scene: '{scene.name}'</color>");
        foreach (GameObject rootObj in scene.GetRootGameObjects())
        {

            Debug.Log($"<color=magenta>  Root object '{rootObj.name}' isActive: {rootObj.activeSelf}</color>");
        }
        
    // Set the newly loaded scene as the active scene
    SceneManager.SetActiveScene(scene);
    Debug.Log($"<color=cyan>GameManager: Active scene has been set to: '{scene.name}'.</color>");

    // Ensure the root objects of the NEWLY LOADED and now active scene are truly active.
    Debug.Log($"<color=lime>GameManager: Ensuring root objects in CURRENT scene '{scene.name}' are active...</color>");
    bool activatedAnyRoot = false; // Flag to check if we activated anything

    // Iterate through all top-level GameObjects in the scene
    foreach (GameObject rootObj in scene.GetRootGameObjects())
    {
        // Check if the current root object is inactive
        if (!rootObj.activeSelf)
        {
           
            if (!rootObj.CompareTag("Player") && rootObj.name != "Main Camera") // Checking for "Player" tag and "Main Camera" name
            {
                rootObj.SetActive(true); // Explicitly set the GameObject to active
                Debug.Log($"<color=lime>GameManager: ACTIVATED INACTIVE ROOT OBJECT: '{rootObj.name}' in scene '{scene.name}'</color>");
                activatedAnyRoot = true; // Mark that we activated at least one object
            }
        }
    }

    // Log a summary based on whether any objects were activated
    if (activatedAnyRoot)
    {
        Debug.Log($"<color=lime>GameManager: One or more root objects in '{scene.name}' were initially inactive and have been activated.</color>");
    }
    else
    {
        Debug.Log($"<color=lime>GameManager: All relevant root objects in '{scene.name}' were already active.</color>");
    }


            if (!_isBacktracking && !string.IsNullOrEmpty(_currentRoomSceneName) && _currentRoomSceneName != scene.name)
        {
            _roomHistoryStack.Push(_currentRoomSceneName);
            Debug.Log($"GameManager: Pushed '{_currentRoomSceneName}' to history stack. Stack size: {_roomHistoryStack.Count}"); // ADDED: Debug log
        }

            _currentRoomSceneName = scene.name;
            _isBacktracking = false;
            Debug.Log($"GameManager: Current room updated to: {_currentRoomSceneName}"); // ADDED: Debug log

            GameObject playerToMove = null;
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log("GameManager: Found " + allPlayers.Length + " GameObjects tagged 'Player' in the scene."); // ADDED: Debug log

            int duplicatePlayerCount = 0;
            foreach (GameObject p in allPlayers)
            {
                // The persistent player should have the PlayerPersist script
                if (p.GetComponent<PlayerPersist>() != null)
                {
                    if (playerToMove == null)
                    {
                        playerToMove = p; // This is our primary, persistent player
                        Debug.Log("GameManager: Identified PERSISTENT Player (with PlayerPersist script) at: " + p.transform.position + ", Instance ID: " + p.GetInstanceID()); // ADDED: Debug log
                    }
                    else
                    {
                        // This scenario should ideally be handled by PlayerPersist itself, but added here for redundancy.
                        Debug.LogWarning("GameManager: Found MORE THAN ONE Player with PlayerPersist script. Destroying this extra one: " + p.name + ", Instance ID: " + p.GetInstanceID()); // ADDED: Debug log
                        Destroy(p);
                        duplicatePlayerCount++;
                    }
                }
                else // This is a Player tagged GameObject WITHOUT PlayerPersist, indicating a newly spawned clone
                {
                    Debug.LogWarning("GameManager: Found a Player tagged 'Player' at " + p.transform.position + ", Instance ID: " + p.GetInstanceID() + ", but it does NOT have the PlayerPersist script. This is a newly spawned duplicate. Destroying it."); // ADDED: Debug log
                    Destroy(p); // Destroy the clone
                    duplicatePlayerCount++;
                }
            }

            if (duplicatePlayerCount > 0)
            {
                Debug.LogWarning("GameManager: " + duplicatePlayerCount + " duplicate Player(s) were found and destroyed during scene load. Please remove any scripts or scene setups that instantiate new players in scenes other than your initial starting scene."); // ADDED: Debug log
            }

            // Now, proceed with the identified persistent player
            if (playerToMove == null) // This means even the persistent one wasn't found or was somehow destroyed
            {
                Debug.LogError("GameManager: OnSceneLoaded: Persistent Player GameObject with tag 'Player' NOT FOUND after all checks. Player cannot be repositioned. Ensure the ONLY initial Player in your FIRST scene has the 'Player' tag, is active, and has the PlayerPersist script."); // ADDED: Error log
                return;
            }
            else
            {
                Debug.Log("GameManager: Proceeding to reposition the persistent Player at: " + playerToMove.transform.position); // ADDED: Debug log

                string spawnPointToFind = string.IsNullOrEmpty(_nextPlayerSpawnPointName) ? "PlayerSpawn" : _nextPlayerSpawnPointName;
                Debug.Log("GameManager: OnSceneLoaded: Looking for spawn point named: '" + spawnPointToFind + "'"); // ADDED: THIS IS THE CRITICAL LOG

                Transform playerSpawnPoint = GameObject.Find(spawnPointToFind)?.transform; // Using null-conditional operator for cleaner code

                if (playerSpawnPoint == null)
                {
                    Debug.LogError("GameManager: OnSceneLoaded: Spawn point named '" + spawnPointToFind + "' NOT FOUND in scene: " + scene.name + ". Player will stay at old position. Check: 1) Exact name match in target scene, 2) Spawn point is active in target scene."); // ADDED: Error log
                }
                else
                {
                    Debug.Log("GameManager: OnSceneLoaded: Spawn point '" + spawnPointToFind + "' found at position: " + playerSpawnPoint.position); // ADDED: Debug log
                    playerToMove.transform.position = playerSpawnPoint.position; // Use playerToMove here
                    Debug.Log("GameManager: Player repositioned to: " + playerToMove.transform.position + " (confirmed)."); // ADDED: Debug log
                    _nextPlayerSpawnPointName = ""; // Clear for next transition
                }
            }

        }

    public void RestartGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        Debug.Log("Restart game button pressed");
        Time.timeScale = 1f;
        int initialSceneCount = SceneManager.sceneCount;
        for (int i = 0; i < initialSceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name != gameObject.scene.name && s.IsValid() && s.isLoaded)
            {
                Debug.Log("GameManager: Unloading Scene '{s.name}' during Restart");
                SceneManager.UnloadSceneAsync(s);
            }

        }
        _previousActiveScene = new Scene();
        Debug.Log("<color=orange>Gamemanager: _previoussceneloaded reset for new game</color");

        _roomHistoryStack.Clear(); // Clears the history
        _currentRoomSceneName = ""; // Resets current room
        _roomClearedStates.Clear(); // Clears all room cleared states


        InitializeAvailableRooms(); // Re-populates available rooms

        if (allRoomScenes.Count > 0) // ADDED: Check to prevent error if allRoomScenes is empty
        {
            // When restarting, load the first scene from your allRoomScenes list and default spawn point
            LoadAndPlacePlayer(allRoomScenes[0], "PlayerSpawn", "");
        }
        else
        {
            Debug.LogError("GameManager: 'allRoomScenes' list is empty. Cannot restart game."); // ADDED: Error log
        }
    }

    void InitializeAvailableRooms()
    {
        availableRoomScenes.Clear(); // Clears the room already there just in case there are some extra ones from the previous load
        Debug.Log("GameManager: Initializing rooms. allRoomScenes.Count from Inspector: " + allRoomScenes.Count); // ADDED: Debug log

        foreach (string roomName in allRoomScenes)
        {
            // --- CHANGED: Initialize room state to not cleared if not already tracked (MOVED INSIDE LOOP) ---
            if (!_roomClearedStates.ContainsKey(roomName))
            {
                _roomClearedStates.Add(roomName, false); // Rooms start as not cleared
            }
            // --- END CHANGED ---

            availableRoomScenes.Add(roomName);
            Debug.Log("GameManager: Added room to availableRoomScenes: " + roomName); // ADDED: Debug log
        }
        Debug.Log("GameManager: Available rooms initialized. Total: " + availableRoomScenes.Count + " rooms."); // ADDED: Debug log

        if (availableRoomScenes.Count == 0) // ADDED: Check if list is empty
        {
            Debug.LogError("GameManager: 'allRoomScenes' list in the Inspector is EMPTY after foreach! Please add scene names and ensure scenes are in Build Settings."); // ADDED: Error log
        }
    }

    public string GetNextRandomRoom() // 
    {
        Debug.Log("GameManager: GetNextRandomRoom called. availableRoomScenes.Count at start: " + availableRoomScenes.Count); // ADDED: Debug log
        if (availableRoomScenes.Count == 0)
        {
            Debug.Log("GameManager: availableRoomScenes count is 0. Attempting to re-initialize rooms from GetNextRandomRoom."); // ADDED: Debug log
            InitializeAvailableRooms();
        }

        if (availableRoomScenes.Count == 0)
        {
            Debug.LogError("GameManager: FATAL: No available rooms even after re-initialization inside GetNextRandomRoom! Returning empty string. Check 'allRoomScenes' list in Inspector AND Build Settings."); // ADDED: Error log
            return "";
        }

        int randomIndex = Random.Range(0, availableRoomScenes.Count);
        string chosenRoom = availableRoomScenes[randomIndex];
        availableRoomScenes.RemoveAt(randomIndex);
        Debug.Log("GameManager: Chosen room: " + chosenRoom + ". Remaining rooms in cycle: " + availableRoomScenes.Count);
        return chosenRoom;
    }


    public void LoadAndPlacePlayer(string sceneName, string spawnPointName, string enteredSpawnPointName = "")
    {
        _nextPlayerSpawnPointName = spawnPointName;
        _lastEnteredDoorName = enteredSpawnPointName;
        Debug.Log("GameManager: Preparing to load scene: '" + sceneName + "' and spawn at: '" + spawnPointName + "'"); // ADDED: Debug log
        _previousActiveScene = SceneManager.GetActiveScene();
        StartCoroutine(LoadSceneAdditiveAsync(sceneName));
    }
    private IEnumerator LoadSceneAdditiveAsync(string sceneName)
    {
        Scene targetScene = SceneManager.GetSceneByName(sceneName);
        if (targetScene.isLoaded)
        {
            Debug.Log("GameManager: Scene '{sceneName}' is already loaded. Reactivating it.");
            OnSceneLoaded(targetScene, LoadSceneMode.Additive);
            yield break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


    public void SetCurrentRoomCleared(bool isCleared)
    {
        if (!string.IsNullOrEmpty(_currentRoomSceneName))
        {
            _roomClearedStates[_currentRoomSceneName] = isCleared;
            Debug.Log($"GameManager: Room '{_currentRoomSceneName}' cleared state set to: {isCleared}");
        }
        else
        {
            Debug.LogWarning("GameManager: Cannot set room cleared state; _currentRoomSceneName is empty."); // ADDED: Warning log
        }
    }


    public bool IsRoomCleared(string roomSceneName)
    {
        if (_roomClearedStates.ContainsKey(roomSceneName))
        {
            return _roomClearedStates[roomSceneName];
        }
        Debug.LogWarning($"GameManager: Room '{roomSceneName}' not found in cleared states dictionary. Defaulting to not cleared."); // ADDED: Warning log
        return false; // Default to not cleared if not tracked
    }

    public void GoBackToLastRoom(string spawnPointInPreviousRoom)
    {
        if (_roomHistoryStack.Count > 0) // Check if there's a room to go back to
        {
            string roomToGoBackTo = _roomHistoryStack.Pop(); // Get the last room from the stack
            _isBacktracking = true; // Set flag so OnSceneLoaded knows we are going back
            Debug.Log($"GameManager: Attempting to go back to room from history stack: '{roomToGoBackTo}' via spawn: '{spawnPointInPreviousRoom}'. Stack size after pop: {_roomHistoryStack.Count}"); // ADDED: Debug log
            LoadAndPlacePlayer(roomToGoBackTo, spawnPointInPreviousRoom, spawnPointInPreviousRoom);
        }
        else
        {
            Debug.LogWarning("GameManager: Room history stack is empty. Cannot go back further. Loading a random room instead."); // ADDED: Warning log
            // Fallback to a random room if no history, make sure GetNextRandomRoom is called
            LoadAndPlacePlayer(GetNextRandomRoom(), "PlayerSpawn");
        }
    }
}
