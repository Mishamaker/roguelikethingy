    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.Collections.Generic;

public class GameManager : MonoBehaviour

{
    public List<string> allRoomScenes = new List<string>();
    private List<string> availableRoomScenes = new List<string>();
    private string _nextPlayerSpawnPointName;
    public GameObject gameOverPanel;
    public static GameManager Instance;
    private Stack<string> _roomHistoryStack = newStack<string>;
    private string _currentRoomSceneName;
    private Directory<string, bool> _roomClearedStates = new Directory<string, bool>();
    void Awake()
    {   SceneManager.sceneLoaded += OnSceneLoaded;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeAvailableRooms();

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {

    }
    public void GameOver()
    {

        gameOverPanel.SetActive(true); // Show the game over panel

        Time.timeScale = 0f; // 
        Debug.Log("Game Over! Time scaled to 0.");
    }

    // Call this method when the Restart button is pressed
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("GameManager: OnSceneLoaded called for scene: " + scene.name);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            string spawnPointToFind = string.IsNullOrEmpty(_nextPlayerSpawnPointName) ? "PlayerSpawn" : _nextPlayerSpawnPointName;
            Transform playerSpawnPoint = GameObject.Find(spawnPointToFind)?.transform;
            if (playerSpawnPoint != null)
            {
                Debug.Log("SpawnPointFound");
                player.transform.position = playerSpawnPoint.position;
                Debug.Log("GameManager: Player repositioned to: " + playerSpawnPoint.position + " using spawn point: " + spawnPointToFind);
                _nextPlayerSpawnPointName = "";
            }
            else
            {
                Debug.Log("SpawnPointNotFound!!");
            }


        }
        else
        {
            Debug.Log("PLayerNotfOundd");
        }
    } 
    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        Debug.Log("Restart game button pressed");
        Time.timeScale = 1f; // 
                             // Get the current scene's index and load it again
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        InitializeAvailableRooms();
    }
    void InitializeAvailableRooms()
    {
        availableRoomScenes.Clear();//clears the room already there just in case there are some extra ones from the previous load
        foreach (string roomName in allRoomScenes)
        {
            availableRoomScenes.Add(roomName);

        }
        Debug.Log("GameManage: avaliable rooms initialized. Total:" + availableRoomScenes.Count + " rooms");
    }
    public string GetNextRoom()
    {
        if (availableRoomScenes.Count == 0)
        {
            InitializeAvailableRooms();
        }
        int randomIndex = Random.Range(0, availableRoomScenes.Count);
        string chosenRoom = availableRoomScenes[randomIndex];
        availableRoomScenes.RemoveAt(randomIndex);
        Debug.Log("GameManager: Chosen room: " + chosenRoom + ". Remaining rooms in cycle: " + availableRoomScenes.Count);
        return chosenRoom;

    }
    public void LoadAndPlacePlayer(string sceneName, string spawnPointName)
    {
        _nextPlayerSpawnPointName = spawnPointName;
        SceneManager.LoadScene(sceneName);
    }

}
