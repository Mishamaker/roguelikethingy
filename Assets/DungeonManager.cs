using UnityEngine;
using System.Collections.Generic;
using System;

public class DungeonManager : MonoBehaviour
{
    [Header("Room Prefabs")] 
    public GameObject[] roomPrefabs; 

    
    public GameObject[] standardRoomPrefabs;
    public GameObject[] treasureRoomPrefabs;
    public GameObject[] bossRoomPrefabs;
    [Header("Dungeon Generation Settings")]
    [Tooltip("The size of the dungeon grid, like 9*9")]
    public int dungeonGridSize = 9;
    [Tooltip("World size of a single room in prefab")]
    public float roomWorldSize = 20f;
    [SerializeField]
    private Room[,] dungeonGrid;

    [Header("Room Prefabs")]
    public GameObject roomPrefab_Start;
    public GameObject roomPrefab_Normal_FourDoors;

    [Header("Player Settings")]
    public GameObject playerPrefab;
    private GameObject currentPlayerInstance;
    [Tooltip("The Current (X,Y) position of the player on the dungeon Grid")]
    public Vector2Int currentGridPosition;
    public static event Action<GameObject> OnPlayerSpawned;
    
    private GameObject currentActiveRoomObject;
    private Transform spawnPoint;
       public float blockedCellPercentage = 0.2f; 


    public static DungeonManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDungeonGrid();
    }

    void InitializeDungeonGrid()
{
    Debug.Log("Initializing Dungeon Grid...");

    dungeonGrid = new Room[dungeonGridSize, dungeonGridSize];

    // 1. Initialize all grid cells to EMPTY first
    for (int x = 0; x < dungeonGridSize; x++)
    {
        for (int y = 0; y < dungeonGridSize; y++)
        {
            dungeonGrid[x, y] = new Room(RoomType.Empty, null, RoomDoors.None); // Initialize as Empty
        }
    }

    
 
    for (int x = 0; x < dungeonGridSize; x++)
    {
        for (int y = 0; y < dungeonGridSize; y++)
        {
           
            int startX = dungeonGridSize / 2;
            int startY = dungeonGridSize / 2;
            if (x == startX && y == startY) continue; // Skip start room
            // Skip immediate neighbors of start room to ensure initial paths
            if ((Mathf.Abs(x - startX) <= 1 && y == startY) || (Mathf.Abs(y - startY) <= 1 && x == startX)) continue; 

            if (UnityEngine.Random.value < blockedCellPercentage)
            {
                dungeonGrid[x, y] = new Room(RoomType.Blocked);
            }
        }
    }

    Debug.Log("Random Blocked cells placed.");

   
    int startRoomX = dungeonGridSize / 2;
    int startRoomY = dungeonGridSize / 2;

    dungeonGrid[startRoomX, startRoomY] = new Room(RoomType.Start, roomPrefab_Start, RoomDoors.None);
    dungeonGrid[startRoomX, startRoomY].worldPosition = new Vector2(startRoomX * roomWorldSize, startRoomY * roomWorldSize);
    dungeonGrid[startRoomX, startRoomY].visited = true;
    currentGridPosition = new Vector2Int(startRoomX, startRoomY);
    Debug.Log($"Start Room set at ({startRoomX},{startRoomY}).");

  
    EnsureDungeonConnectivity(); 
    Debug.Log("Dungeon connectivity ensured. Empty cells around paths are now Normal.");

    
    dungeonGrid[startRoomX, startRoomY].roomDoors = CalculateRoomDoors(new Vector2Int(startRoomX, startRoomY));
    Debug.Log($"[InitializeDungeonGrid] Start Room at ({startRoomX},{startRoomY}) final calculated doors: {dungeonGrid[startRoomX, startRoomY].roomDoors}");

    if (playerPrefab != null)
    {
        currentPlayerInstance = Instantiate(playerPrefab);
        OnPlayerSpawned?.Invoke(currentPlayerInstance);
        Debug.Log("Player instantiated.");
    }
    else
    {
        Debug.LogWarning("Player Prefab is not assigned in DungeonManager!");
    }
    
    
    LoadRoomAtGridPosition(currentGridPosition, GetSpawnPointNameForDirection(RoomDoors.None));
    Debug.Log("Dungeon initialized and Start Room loaded.");
}
void EnsureDungeonConnectivity()
{
    Queue<Vector2Int> queue = new Queue<Vector2Int>();
    HashSet<Vector2Int> visitedCells = new HashSet<Vector2Int>();

    // Start flood fill from the current player position (which is the start room)
    queue.Enqueue(currentGridPosition);
    visitedCells.Add(currentGridPosition);

    int cellsToGenerate = Mathf.Max(1, dungeonGridSize * dungeonGridSize / 4); // Example: generate at least 25% of grid as normal rooms
    int generatedCells = 0;

    while (queue.Count > 0 && generatedCells < cellsToGenerate)
    {
        Vector2Int current = queue.Dequeue();

        // If the current cell is Empty, make it Normal and assign a prefab (if not already Start or Normal)
        if (dungeonGrid[current.x, current.y].roomType == RoomType.Empty)
        {
            dungeonGrid[current.x, current.y].roomType = RoomType.Normal;
            dungeonGrid[current.x, current.y].roomPrefab = roomPrefab_Normal_FourDoors; // Assign a generic prefab
            dungeonGrid[current.x, current.y].worldPosition = new Vector2(current.x * roomWorldSize, current.y * roomWorldSize);

      
            dungeonGrid[current.x, current.y].roomDoors = CalculateRoomDoors(current);
        
        }
        generatedCells++;

        // Get valid neighbors (within bounds, not blocked, and not yet visited for pathing)
        List<Vector2Int> neighbors = GetValidNeighbors(current);
        foreach (Vector2Int neighbor in neighbors)
        {
            if (!visitedCells.Contains(neighbor) && dungeonGrid[neighbor.x, neighbor.y].roomType != RoomType.Blocked)
            {
                visitedCells.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }
    }
    Debug.Log($"[DungeonGeneration] Connectivity ensured. Generated {generatedCells} normal rooms.");
}


List<Vector2Int> GetValidNeighbors(Vector2Int gridPos)
{
    List<Vector2Int> neighbors = new List<Vector2Int>();

    // North
    if (gridPos.y + 1 < dungeonGridSize) neighbors.Add(new Vector2Int(gridPos.x, gridPos.y + 1));
    // East
    if (gridPos.x + 1 < dungeonGridSize) neighbors.Add(new Vector2Int(gridPos.x + 1, gridPos.y));
    // South
    if (gridPos.y - 1 >= 0) neighbors.Add(new Vector2Int(gridPos.x, gridPos.y - 1));
    // West
    if (gridPos.x - 1 >= 0) neighbors.Add(new Vector2Int(gridPos.x - 1, gridPos.y));

    return neighbors;
}
 void LoadRoomAtGridPosition(Vector2Int gridPos, string spawnPointName)
    {
        Debug.Log($"[LoadRoom] Attempting to load room at grid: {gridPos.x},{gridPos.y} with spawn point: {spawnPointName}");
        Room roomToLoad = dungeonGrid[gridPos.x, gridPos.y];

        if (roomToLoad == null)
        {
            Debug.LogError("roomToLoad is NULL! This should not happen if dungeonGrid is initialized correctly.");
            return;
        }

        if (roomToLoad.roomPrefab == null)
        {
            Debug.LogError($"Room prefab is NULL for room at ({gridPos.x},{gridPos.y}). Make sure it's assigned in the DungeonManager Inspector or during room data setup!");
            return;
        }

        if (currentActiveRoomObject != null && currentActiveRoomObject != roomToLoad.instantiatedRoomObject)
        {
            Debug.Log($"[LoadRoom] Deactivating previous room: {currentActiveRoomObject.name}");
            currentActiveRoomObject.SetActive(false);
        }

        if (roomToLoad.instantiatedRoomObject != null)
        {
            Debug.Log($"[LoadRoom] Room already instantiated: {roomToLoad.instantiatedRoomObject.name}. Activating it.");
            roomToLoad.instantiatedRoomObject.SetActive(true);
            currentActiveRoomObject = roomToLoad.instantiatedRoomObject;
            Debug.Log($"[LoadRoom] currentActiveRoomObject set to (existing room): {currentActiveRoomObject.name}");
        }
        else
        {
            Debug.Log($"[LoadRoom] roomToLoad.instantiatedRoomObject is NULL. Instantiating new room from prefab: {roomToLoad.roomPrefab.name}");
             Vector3 roomWorldPosition = new Vector3(roomToLoad.worldPosition.x, roomToLoad.worldPosition.y, 0f); // Ensure Z is handled
        int randomIndex = UnityEngine.Random.Range(0, roomPrefabs.Length); 

        
        GameObject chosenRoomPrefab = roomPrefabs[randomIndex];

   
        GameObject newRoomObject = Instantiate(chosenRoomPrefab, roomWorldPosition , Quaternion.identity);

            if (newRoomObject == null)
            {
                Debug.LogError($"[LoadRoom ERROR] Instantiate returned NULL for prefab: {roomToLoad.roomPrefab.name}. Room could not be created! Cannot place player.");
                return;
            }

            roomToLoad.instantiatedRoomObject = newRoomObject;
            currentActiveRoomObject = newRoomObject;
            roomToLoad.visited = true;

            Debug.Log($"[LoadRoom] New room instantiated and assigned: {newRoomObject.name}. currentActiveRoomObject set to (new room): {currentActiveRoomObject.name}. Player will be placed.");
        }

        if (currentActiveRoomObject != null)
        {
            Debug.Log($"[LoadRoom Debug] Room to load at {gridPos.x},{gridPos.y} has roomType: {roomToLoad.roomType}, roomDoors data: {roomToLoad.roomDoors}");

            RoomController roomController = currentActiveRoomObject.GetComponent<RoomController>();
            if (roomController != null)
            {
                roomController.SetupDoors(roomToLoad); 
                Debug.Log($"[LoadRoom Debug] RoomController found on {currentActiveRoomObject.name}. Setting up doors based on data: {roomToLoad.roomDoors}");
            }
            else
            {
                Debug.LogWarning($"[LoadRoom] Room object {currentActiveRoomObject.name} is missing a RoomController! Doors might not update visually.");
            }

            PlacePlayerInCurrentRoom(spawnPointName);
            Debug.Log("Player will be placed in current room after door setup.");
            
            
            if (roomController != null) 
            {
                // Call the method to check for enemies and lock/unlock doors
                CheckRoomForEnemiesAndLockDoors(roomController, currentActiveRoomObject); 
                Debug.Log($"[LoadRoom] Called CheckRoomForEnemiesAndLockDoors for {currentActiveRoomObject.name}.");
            }
            // ------------------------------------
        }
        else
        {
            Debug.LogError("[LoadRoom Final Error] currentActiveRoomObject is NULL after instantiation/activation attempt. Cannot proceed with RoomController setup or player placement.");
            return; 
        }
    }
     void CheckRoomForEnemiesAndLockDoors(RoomController roomController, GameObject roomObject)
    {
       
        int enemiesFound = 0;
        foreach (Transform child in roomObject.transform)
        {
            
            if (child.CompareTag("Enemy"))
            {
                
                roomController.RegisterEnemy(child.gameObject);
                enemiesFound++;
            }
        }

        if (enemiesFound > 0)
        {
            roomController.LockAllActiveDoors();
            Debug.Log($"[DungeonManager] Room has {enemiesFound} enemies. Doors locked.");
        }
        else
        {
            roomController.UnlockAllActiveDoors(); // Ensure doors are unlocked if no enemies
            Debug.Log($"[DungeonManager] No enemies in room. Doors unlocked.");
        }
    }
   void PlacePlayerInCurrentRoom(string spawnPointName)
{
    Debug.Log($"[PlacePlayer] Attempting to place player in room '{ (currentActiveRoomObject != null ? currentActiveRoomObject.name : "NULL_ROOM_OBJECT") }' using spawn point name: '{spawnPointName}'");

    if (currentPlayerInstance == null) // Add a null check for player instance
    {
        
        Debug.LogError("[PlacePlayer Error] currentPlayerInstance is NULL. Cannot place player.");
        return;
    }

    if (currentActiveRoomObject != null)
    {
        spawnPoint = currentActiveRoomObject.transform.Find(spawnPointName);
        Vector3 playerTargetPosition; // Use Vector3 to handle Z explicitly

        if (spawnPoint == null)
        {
            Debug.LogError($"<color=red>Spawn point '{spawnPointName}' NOT found as a child of '{currentActiveRoomObject.name}'.</color> Placing player at room center as fallback.");
            
            // Calculate room center, then set a consistent Z
            playerTargetPosition = new Vector3(
                currentGridPosition.x * roomWorldSize + (roomWorldSize / 2f),
                currentGridPosition.y * roomWorldSize + (roomWorldSize / 2f),
                -0.1f // <--- IMPORTANT: Set your desired fixed Z-position here!
            );
        }
        else
        {
            playerTargetPosition = spawnPoint.position;
            // <--- IMPORTANT: Override the spawnPoint's Z with your desired fixed Z-position!
            playerTargetPosition.z = -0.1f; // Adjust this value as needed for your scene
            Debug.Log($"<color=green>Player spawned at '{spawnPoint.name}' (World Pos: {spawnPoint.position}) in '{currentActiveRoomObject.name}'.</color>");
        }

        currentPlayerInstance.transform.position = playerTargetPosition;
        Debug.Log($"Player final placed at: {currentPlayerInstance.transform.position}");
    }
    else
    {
        Debug.LogError("[PlacePlayer Error] currentActiveRoomObject is NULL. Cannot place player.");
    }
}

    private RoomDoors GetOppositeDirection(RoomDoors direction)
    {
        switch (direction)
        {
            case RoomDoors.North: return RoomDoors.South;
            case RoomDoors.East: return RoomDoors.West;
            case RoomDoors.South: return RoomDoors.North;
            case RoomDoors.West: return RoomDoors.East;
            default: return RoomDoors.None;
        }
    }


public void MovePlayer(RoomDoors exitDirection)
{
    Vector2Int newGridPos = currentGridPosition;

    //  Calculate the potential new grid position based on exitDirection
    switch (exitDirection)
    {
        case RoomDoors.East: newGridPos.x += 1; break;
        case RoomDoors.West: newGridPos.x -= 1; break;
        case RoomDoors.North: newGridPos.y += 1; break;
        case RoomDoors.South: newGridPos.y -= 1; break;
        default: Debug.LogWarning("DungeonManager: No valid exit direction found."); return;
    }

    
    if (newGridPos.x < 0 || newGridPos.x >= dungeonGridSize ||
        newGridPos.y < 0 || newGridPos.y >= dungeonGridSize)
    {
        Debug.LogWarning($"Attempted to move out of dungeon bounds! Current: {currentGridPosition}, Tried: {newGridPos}");
        return; // Stop movement if out of bounds
    }

    if (dungeonGrid[newGridPos.x, newGridPos.y].roomType == RoomType.Blocked)
    {
        Debug.Log("Player is trying to enter a blocked room. Movement stopped.");
        return; // Prevent movement into Blocked cells
    }

    
    RoomDoors entryDirectionIntoNewRoom = GetOppositeDirection(exitDirection);
          Debug.Log($"[MovePlayer Debug] Moving from {currentGridPosition} (Type: {dungeonGrid[currentGridPosition.x, currentGridPosition.y].roomType}) via {exitDirection}. Target grid: {newGridPos}. RoomType at target: {dungeonGrid[newGridPos.x, newGridPos.y].roomType}");
        //  Handle movement into an Empty cell (generating a new room)
        if (dungeonGrid[newGridPos.x, newGridPos.y].roomType == RoomType.Empty)
        {
            Debug.Log($"[MovePlayer - New Room] Processing empty cell at {newGridPos}.");

            dungeonGrid[newGridPos.x, newGridPos.y].roomType = RoomType.Normal;
            dungeonGrid[newGridPos.x, newGridPos.y].roomPrefab = roomPrefab_Normal_FourDoors;// Still using Start prefab for Normal rooms

            RoomDoors generatedDoors = CalculateRoomDoors(newGridPos);
            generatedDoors |= entryDirectionIntoNewRoom;
            dungeonGrid[newGridPos.x, newGridPos.y].roomDoors = generatedDoors;


            Debug.Log($"[MovePlayer - New Room] Before worldPosition assignment: dungeonGrid[{newGridPos.x},{newGridPos.y}].worldPosition = {dungeonGrid[newGridPos.x, newGridPos.y].worldPosition}");

            Vector2 calculatedWorldPos = new Vector2(newGridPos.x * roomWorldSize, newGridPos.y * roomWorldSize);
            Debug.Log($"[MovePlayer - New Room] Calculated worldPosition: {calculatedWorldPos} (from grid {newGridPos} and roomWorldSize {roomWorldSize})");

            dungeonGrid[newGridPos.x, newGridPos.y].worldPosition = calculatedWorldPos; // Assign the calculated value

            Debug.Log($"[MovePlayer - New Room] After worldPosition assignment: dungeonGrid[{newGridPos.x},{newGridPos.y}].worldPosition = {dungeonGrid[newGridPos.x, newGridPos.y].worldPosition}");


            Debug.Log($"[MovePlayer Debug] New room at {newGridPos.x},{newGridPos.y} has calculated doors: {generatedDoors}");
        }
        else
        {
            Debug.Log("ts not empty");
    }
    
    currentGridPosition = newGridPos;

   
    string spawnPointNameInNewRoom = GetSpawnPointNameForDirection(entryDirectionIntoNewRoom);

    //  Load the room at the new grid position (this handles instantiation/activation and calls RoomController.SetupDoors)
    LoadRoomAtGridPosition(currentGridPosition, spawnPointNameInNewRoom);
    Debug.Log($"Room at {currentGridPosition.x},{currentGridPosition.y} is now loaded."); // Debug Log
}

    private string GetSpawnPointNameForDirection(RoomDoors direction)
    {
        switch (direction)
        {
            case RoomDoors.North: return "SpawnPoint_North";
            case RoomDoors.East: return "SpawnPoint_East";
            case RoomDoors.South: return "SpawnPoint_South";
            case RoomDoors.West: return "SpawnPoint_West";
            case RoomDoors.None: return "PlayerSpawn_Start";
            default: return "PlayerSpawn_Start";
        }
    }
    private RoomDoors CalculateRoomDoors(Vector2Int roomGridPos)
{
    RoomDoors generatedDoors = RoomDoors.None;

    // --- North ---
    Vector2Int northNeighborPos = new Vector2Int(roomGridPos.x, roomGridPos.y + 1);
    if (northNeighborPos.y < dungeonGridSize && // Check upper Y boundary
        northNeighborPos.x >= 0 && northNeighborPos.x < dungeonGridSize && // Check X boundaries
        dungeonGrid[northNeighborPos.x, northNeighborPos.y].roomType != RoomType.Blocked)
    {
        Debug.Log($"Room at {roomGridPos}: North is not blocked.");
        generatedDoors |= RoomDoors.North;
    }

    // --- East ---
    Vector2Int eastNeighborPos = new Vector2Int(roomGridPos.x + 1, roomGridPos.y);
    if (eastNeighborPos.x < dungeonGridSize && // Check right X boundary
        eastNeighborPos.y >= 0 && eastNeighborPos.y < dungeonGridSize && // Check Y boundaries
        dungeonGrid[eastNeighborPos.x, eastNeighborPos.y].roomType != RoomType.Blocked)
    {
        Debug.Log($"Room at {roomGridPos}: East is not blocked.");
        generatedDoors |= RoomDoors.East;
    }

    // --- South ---
    Vector2Int southNeighborPos = new Vector2Int(roomGridPos.x, roomGridPos.y - 1);
    if (southNeighborPos.y >= 0 && // Check lower Y boundary
        southNeighborPos.x >= 0 && southNeighborPos.x < dungeonGridSize && // Check X boundaries
        dungeonGrid[southNeighborPos.x, southNeighborPos.y].roomType != RoomType.Blocked)
    {
        Debug.Log($"Room at {roomGridPos}: South is not blocked.");
        generatedDoors |= RoomDoors.South;
    }

    // --- West ---
    Vector2Int westNeighborPos = new Vector2Int(roomGridPos.x - 1, roomGridPos.y);
    if (westNeighborPos.x >= 0 && // Check left X boundary
        westNeighborPos.y >= 0 && westNeighborPos.y < dungeonGridSize && // Check Y boundaries
        dungeonGrid[westNeighborPos.x, westNeighborPos.y].roomType != RoomType.Blocked)
    {
        Debug.Log($"Room at {roomGridPos}: West is not blocked.");
        generatedDoors |= RoomDoors.West;
    }

    return generatedDoors;
}
    

    private void OnDrawGizmos()
    {
        if (dungeonGrid != null)
        {
            for (int x = 0; x < dungeonGridSize; x++)
            {
                for (int y = 0; y < dungeonGridSize; y++)
                {
                    Vector3 cellCenter = new Vector3(
                        x * roomWorldSize + (roomWorldSize / 2f),
                        y * roomWorldSize + (roomWorldSize / 2f),
                        0
                    );

                    Vector3 cubeDimensions = new Vector3(roomWorldSize, roomWorldSize, 0.1f);
                    Room currentRoom = dungeonGrid[x, y];
                    Gizmos.color = Color.grey;
                    Gizmos.DrawWireCube(cellCenter, cubeDimensions);
                    if (currentRoom.roomType == RoomType.Blocked)
                    {
                        Gizmos.color = Color.red;

                        Gizmos.DrawCube(cellCenter, new Vector3(roomWorldSize, roomWorldSize, 0.1f));
                    }
                    if (dungeonGrid[x, y] != null && dungeonGrid[x, y].roomPrefab != null)
                    {
                        if (dungeonGrid[x, y].visited)
                        {
                            Gizmos.color = Color.blue;
                            Gizmos.DrawCube(cellCenter, new Vector3(roomWorldSize, roomWorldSize, 0.1f));
                        }
                    }
                }
            }
        }
    }
}
