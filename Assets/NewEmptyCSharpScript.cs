using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class DungeonManager : MonoBehaviour
{


    [Header("Dungeon Generation Settings")]
    [ToolTip("The size of the dungeon grid, like 9*9")]
    public int dungeonGridSize = 9;
    [ToolTip("World size of a single room in prefab")]
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
    public static DungeonManager Instance
    {
        get; private set;
    }
    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject); return;

        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitiliazeDungeonGrid();
        SceneManager.sceneLoaded += OnRoomSceneLoaded;
    }
    
} 
