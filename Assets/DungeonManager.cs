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
    

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public GameObject currentPlayerInstance;
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
        dungeonGrid = new Room[dungeonGridSize, dungeonGridSize];

        for (int x = 0; x < dungeonGridSize; x++)
        {
            for (int y = 0; y < dungeonGridSize; y++)
            {
                dungeonGrid[x, y] = new Room(RoomType.Empty, null, RoomDoors.None);
            }
        }

        for (int x = 0; x < dungeonGridSize; x++)
        {
            for (int y = 0; y < dungeonGridSize; y++)
            {
                int startX = dungeonGridSize / 2;
                int startY = dungeonGridSize / 2;
                if (x == startX && y == startY) continue;
                if ((Mathf.Abs(x - startX) <= 1 && y == startY) || (Mathf.Abs(y - startY) <= 1 && x == startX)) continue; 

                if (UnityEngine.Random.value < blockedCellPercentage)
                {
                    dungeonGrid[x, y] = new Room(RoomType.Blocked);
                }
            }
        }

        int startRoomX = dungeonGridSize / 2;
        int startRoomY = dungeonGridSize / 2;

        dungeonGrid[startRoomX, startRoomY] = new Room(RoomType.Start, roomPrefab_Start, RoomDoors.None);
        dungeonGrid[startRoomX, startRoomY].worldPosition = new Vector2(startRoomX * roomWorldSize, startRoomY * roomWorldSize);
        dungeonGrid[startRoomX, startRoomY].visited = true;
        currentGridPosition = new Vector2Int(startRoomX, startRoomY);
    
        EnsureDungeonConnectivity(); 

        dungeonGrid[startRoomX, startRoomY].roomDoors = CalculateRoomDoors(new Vector2Int(startRoomX, startRoomY));

        if (playerPrefab != null)
        {
            currentPlayerInstance = Instantiate(playerPrefab);
            OnPlayerSpawned?.Invoke(currentPlayerInstance);
        }
        
        LoadRoomAtGridPosition(currentGridPosition, GetSpawnPointNameForDirection(RoomDoors.None));
    }
    
    void EnsureDungeonConnectivity()
{
    Queue<Vector2Int> queue = new Queue<Vector2Int>();
    HashSet<Vector2Int> visitedCells = new HashSet<Vector2Int>();

    queue.Enqueue(currentGridPosition);
    visitedCells.Add(currentGridPosition);

    int cellsToGenerate = Mathf.Max(1, dungeonGridSize * dungeonGridSize / 4);
    int generatedCells = 0;
    
    Vector2Int farthestCell = Vector2Int.zero;
    float maxDistance = 0f;

    while (queue.Count > 0 && generatedCells < cellsToGenerate)
    {
        Vector2Int current = queue.Dequeue();

      
        float currentDistance = Vector2Int.Distance(current, currentGridPosition);
        if (currentDistance > maxDistance)
        {
            maxDistance = currentDistance;
            farthestCell = current;
        }

        if (dungeonGrid[current.x, current.y].roomType == RoomType.Empty)
        {
            dungeonGrid[current.x, current.y].roomType = RoomType.Normal;
            
            int randomIndex = UnityEngine.Random.Range(0, standardRoomPrefabs.Length);
            dungeonGrid[current.x, current.y].roomPrefab = standardRoomPrefabs[randomIndex];
        
            dungeonGrid[current.x, current.y].worldPosition = new Vector2(current.x * roomWorldSize, current.y * roomWorldSize);
            dungeonGrid[current.x, current.y].roomDoors = CalculateRoomDoors(current);
        }
        generatedCells++;

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

   
    if (bossRoomPrefabs.Length > 0 && farthestCell != Vector2Int.zero)
    {
        dungeonGrid[farthestCell.x, farthestCell.y].roomType = RoomType.Boss;
        int randomIndex = UnityEngine.Random.Range(0, bossRoomPrefabs.Length);
        dungeonGrid[farthestCell.x, farthestCell.y].roomPrefab = bossRoomPrefabs[randomIndex];
        dungeonGrid[farthestCell.x, farthestCell.y].worldPosition = new Vector2(farthestCell.x * roomWorldSize, farthestCell.y * roomWorldSize);
        dungeonGrid[farthestCell.x, farthestCell.y].roomDoors = CalculateRoomDoors(farthestCell);
    }
}


    List<Vector2Int> GetValidNeighbors(Vector2Int gridPos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (gridPos.y + 1 < dungeonGridSize) neighbors.Add(new Vector2Int(gridPos.x, gridPos.y + 1));
        if (gridPos.x + 1 < dungeonGridSize) neighbors.Add(new Vector2Int(gridPos.x + 1, gridPos.y));
        if (gridPos.y - 1 >= 0) neighbors.Add(new Vector2Int(gridPos.x, gridPos.y - 1));
        if (gridPos.x - 1 >= 0) neighbors.Add(new Vector2Int(gridPos.x - 1, gridPos.y));

        return neighbors;
    }
    
    void LoadRoomAtGridPosition(Vector2Int gridPos, string spawnPointName)
    {
        Room roomToLoad = dungeonGrid[gridPos.x, gridPos.y];

        if (roomToLoad == null)
        {
            return;
        }

        if (roomToLoad.roomPrefab == null)
        {
            return;
        }

        if (currentActiveRoomObject != null && currentActiveRoomObject != roomToLoad.instantiatedRoomObject)
        {
            currentActiveRoomObject.SetActive(false);
        }

        if (roomToLoad.instantiatedRoomObject != null)
        {
            roomToLoad.instantiatedRoomObject.SetActive(true);
            currentActiveRoomObject = roomToLoad.instantiatedRoomObject;
        }
        else
        {
            Vector3 roomWorldPosition = new Vector3(roomToLoad.worldPosition.x, roomToLoad.worldPosition.y, 0f);
            GameObject newRoomObject = Instantiate(roomToLoad.roomPrefab, roomWorldPosition, Quaternion.identity);

            if (newRoomObject == null)
            {
                return;
            }

            roomToLoad.instantiatedRoomObject = newRoomObject;
            currentActiveRoomObject = newRoomObject;
            roomToLoad.visited = true;
        }

        if (currentActiveRoomObject != null)
        {
            RoomController roomController = currentActiveRoomObject.GetComponent<RoomController>();
            if (roomController != null)
            {
                roomController.SetupDoors(roomToLoad); 
            }
            
            PlacePlayerInCurrentRoom(spawnPointName);
            
            if (roomController != null) 
            {
                CheckRoomForEnemiesAndLockDoors(roomController, currentActiveRoomObject); 
            }
        }
        else
        {
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
            if (MusicManager.Instance != null) 
            {
                MusicManager.Instance.SetBattleMusic(true);
            }
            roomController.LockAllActiveDoors();
        }
        else
        {
            if (MusicManager.Instance != null) 
            {
                MusicManager.Instance.SetBattleMusic(false);
            }
            roomController.UnlockAllActiveDoors();
        }
    }
    
    void PlacePlayerInCurrentRoom(string spawnPointName)
    {
        if (currentPlayerInstance == null)
        {
            return;
        }

        if (currentActiveRoomObject != null)
        {
            spawnPoint = currentActiveRoomObject.transform.Find(spawnPointName);
            Vector3 playerTargetPosition;

            if (spawnPoint == null)
            {
                playerTargetPosition = new Vector3(
                    currentGridPosition.x * roomWorldSize + (roomWorldSize / 2f),
                    currentGridPosition.y * roomWorldSize + (roomWorldSize / 2f),
                    -0.1f
                );
            }
            else
            {
                playerTargetPosition = spawnPoint.position;
                playerTargetPosition.z = -0.1f;
            }

            currentPlayerInstance.transform.position = playerTargetPosition;
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

        switch (exitDirection)
        {
            case RoomDoors.East: newGridPos.x += 1; break;
            case RoomDoors.West: newGridPos.x -= 1; break;
            case RoomDoors.North: newGridPos.y += 1; break;
            case RoomDoors.South: newGridPos.y -= 1; break;
            default: return;
        }

        if (newGridPos.x < 0 || newGridPos.x >= dungeonGridSize ||
            newGridPos.y < 0 || newGridPos.y >= dungeonGridSize)
        {
            return;
        }

        if (dungeonGrid[newGridPos.x, newGridPos.y].roomType == RoomType.Blocked)
        {
            return;
        }

        RoomDoors entryDirectionIntoNewRoom = GetOppositeDirection(exitDirection);
        
        if (dungeonGrid[newGridPos.x, newGridPos.y].roomType == RoomType.Empty)
        {
            dungeonGrid[newGridPos.x, newGridPos.y].roomType = RoomType.Normal;
            
            int randomIndex = UnityEngine.Random.Range(0, standardRoomPrefabs.Length);
            dungeonGrid[newGridPos.x, newGridPos.y].roomPrefab = standardRoomPrefabs[randomIndex];
            
            RoomDoors generatedDoors = CalculateRoomDoors(newGridPos);
            generatedDoors |= entryDirectionIntoNewRoom;
            dungeonGrid[newGridPos.x, newGridPos.y].roomDoors = generatedDoors;

            Vector2 calculatedWorldPos = new Vector2(newGridPos.x * roomWorldSize, newGridPos.y * roomWorldSize);
            dungeonGrid[newGridPos.x, newGridPos.y].worldPosition = calculatedWorldPos;
        }
        
        currentGridPosition = newGridPos;

        string spawnPointNameInNewRoom = GetSpawnPointNameForDirection(entryDirectionIntoNewRoom);

        LoadRoomAtGridPosition(currentGridPosition, spawnPointNameInNewRoom);
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

        Vector2Int northNeighborPos = new Vector2Int(roomGridPos.x, roomGridPos.y + 1);
        if (northNeighborPos.y < dungeonGridSize && 
            northNeighborPos.x >= 0 && northNeighborPos.x < dungeonGridSize &&
            dungeonGrid[northNeighborPos.x, northNeighborPos.y].roomType != RoomType.Blocked)
        {
            generatedDoors |= RoomDoors.North;
        }

        Vector2Int eastNeighborPos = new Vector2Int(roomGridPos.x + 1, roomGridPos.y);
        if (eastNeighborPos.x < dungeonGridSize && 
            eastNeighborPos.y >= 0 && eastNeighborPos.y < dungeonGridSize &&
            dungeonGrid[eastNeighborPos.x, eastNeighborPos.y].roomType != RoomType.Blocked)
        {
            generatedDoors |= RoomDoors.East;
        }

        Vector2Int southNeighborPos = new Vector2Int(roomGridPos.x, roomGridPos.y - 1);
        if (southNeighborPos.y >= 0 &&
            southNeighborPos.x >= 0 && southNeighborPos.x < dungeonGridSize &&
            dungeonGrid[southNeighborPos.x, southNeighborPos.y].roomType != RoomType.Blocked)
        {
            generatedDoors |= RoomDoors.South;
        }

        Vector2Int westNeighborPos = new Vector2Int(roomGridPos.x - 1, roomGridPos.y);
        if (westNeighborPos.x >= 0 &&
            westNeighborPos.y >= 0 && westNeighborPos.y < dungeonGridSize &&
            dungeonGrid[westNeighborPos.x, westNeighborPos.y].roomType != RoomType.Blocked)
        {
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