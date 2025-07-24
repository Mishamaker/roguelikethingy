using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class DungeonManager : MonoBehaviour
{


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
    private GameObject currentActiveRoomObject;
    private Transform spawnPoint;
    public static DungeonManager Instance

    {
        get; private set;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep DungeonManager across scene loads if you change scenes later.

        // Initialize the dungeon grid and generate the first floor immediately
        InitialiazeDungeonGrid();
    }
    

    void InitialiazeDungeonGrid()
    {
        dungeonGrid = new Room[dungeonGridSize, dungeonGridSize];
        for (int x = 0; x < dungeonGridSize; x++)
        {

            for (int y = 0; y < dungeonGridSize; y++)
            {
                dungeonGrid[x, y] = new Room(RoomType.Empty);
            }

        }
        int startX = dungeonGridSize / 2;
        int startY = dungeonGridSize / 2;
        dungeonGrid[startX, startY] = new Room(RoomType.Start, roomPrefab_Start, RoomDoors.North | RoomDoors.East | RoomDoors.South | RoomDoors.West);
        dungeonGrid[startX, startY].worldPosition = new Vector2(startX * roomWorldSize, startY * roomWorldSize);
        currentGridPosition = new Vector2Int(startX, startY);

        currentPlayerInstance = Instantiate(playerPrefab);
        LoadRoomAtGridPosition(currentGridPosition);

    }

    void LoadRoomAtGridPosition(Vector2Int gridPos)
    {
        Room roomToLoad = dungeonGrid[gridPos.x, gridPos.y];
        if (roomToLoad == null)
        {
            return;
        }
        if (roomToLoad.instantiatedRoomObject != null)
        {


            if (currentActiveRoomObject != null && currentActiveRoomObject != roomToLoad.instantiatedRoomObject)
            {
                currentActiveRoomObject.SetActive(false);

            }
            roomToLoad.instantiatedRoomObject.SetActive(true);
            currentActiveRoomObject = roomToLoad.instantiatedRoomObject;
            Debug.Log("new room loaded");
            PlacePlayerInCurrentRoom();
            return;
        }
        GameObject newRoomObject = Instantiate(roomToLoad.roomPrefab, roomToLoad.worldPosition, Quaternion.identity);
        roomToLoad.instantiatedRoomObject = newRoomObject;
        currentActiveRoomObject = newRoomObject;
        roomToLoad.visited = true;


    }
    void PlacePlayerInCurrentRoom()
    {


        if (currentActiveRoomObject != null)
        {
            spawnPoint = currentActiveRoomObject.transform.Find("PlayerSpawn");
            if (spawnPoint == null)
            {
                Debug.Log("< color = red > Spawn point not found yo </color>");
                Vector2 roomCenterWorldPos = new Vector2(
            currentGridPosition.x * roomWorldSize + (roomWorldSize / 2f),
            currentGridPosition.y * roomWorldSize + (roomWorldSize / 2f)
                );
        currentPlayerInstance.transform.position = roomCenterWorldPos;
        Debug.Log($"Player placed at room center: {roomCenterWorldPos}");

            }
            else
            {
                currentPlayerInstance.transform.position = spawnPoint.position;
                Debug.Log("<color=green>spawn point found(:</color>");

            }
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
            default: Debug.LogWarning("no exit direction found somewhow ):<");
                return;
                // TO DO (add boundry checks here)
        }
        currentGridPosition = newGridPos;
        if (currentActiveRoomObject != null)
        {
            currentActiveRoomObject.SetActive(false);
        }
        LoadRoomAtGridPosition(currentGridPosition);

    }
    private void OnDrawGizmos()
    {
        if (dungeonGrid != null)
        {
            Gizmos.color = Color.grey;
            for (int x = 0; x < dungeonGridSize; x++)
            {
                for (int y = 0; y < dungeonGridSize; y++)
                {
                    Vector3 center = new Vector3(x * roomWorldSize, y * roomWorldSize, 0);
                    Gizmos.DrawWireCube(center, new Vector3(roomWorldSize+(roomWorldSize / 2f), roomWorldSize+(roomWorldSize / 2f), 0));
                    if (dungeonGrid[x, y] != null && dungeonGrid[x, y].roomPrefab != null)
                    {
                        if (dungeonGrid[x, y].visited)
                        {
                            Gizmos.color = Color.blue;
                            Gizmos.DrawCube(center, new Vector3(roomWorldSize, roomWorldSize, 0.1f));
                        }
                    }
                }
            }
        }
    }
    
} 
