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

            
        }
        else
        {

            
            Destroy(gameObject);
            return;
        }


    }

    void OnDestroy()
    {
        if (Instance == this)
        {

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





    public void RestartGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        Debug.Log("Restart game button pressed");
        Time.timeScale = 1f;
    }
}
