using UnityEngine;
using UnityEngine.SceneManagement;
public class roomTransition : MonoBehaviour
{
    public string targetSpawnPointNameInNextRoom;
    public string correspondingSpawnPointName;
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
{
    if (GameManager.Instance == null)
    {
        Debug.LogError("RoomTransition: GameManager.Instance is null. Cannot proceed with transition.");
        return;
    }

    if (GameManager.Instance._lastEnteredDoorName == correspondingSpawnPointName)
    {
        // This door is the backtracking door
        Debug.Log($"Door '{gameObject.name}' (associated with '{correspondingSpawnPointName}') is acting as the BACKTRACKING door.");
        // Determine the spawn point for the *previous* room based on the current door's correspondingSpawnPointName
        string spawnPointForPreviousRoom = "";

        // Example logic:
        if (correspondingSpawnPointName == "PlayerSpawn_Up")
        {
            spawnPointForPreviousRoom = "PlayerSpawn_Down"; // If you exited UP, you want to re-enter the previous room from its DOWN spawn.
        }
        else if (correspondingSpawnPointName == "PlayerSpawn_Down")
        {
            spawnPointForPreviousRoom = "PlayerSpawn_Up"; // If you exited DOWN, you want to re-enter the previous room from its UP spawn.
        }
        else if (correspondingSpawnPointName == "PlayerSpawn_Left")
        {
            spawnPointForPreviousRoom = "PlayerSpawn_Right"; // If you exited LEFT, you want to re-enter the previous room from its RIGHT spawn.
        }
        else if (correspondingSpawnPointName == "PlayerSpawn_Right")
        {
            spawnPointForPreviousRoom = "PlayerSpawn_Left"; // If you exited RIGHT, you want to re-enter the previous room from its LEFT spawn.
        }
        // Add more else-if for any other specific spawn point names you might have

        if (string.IsNullOrEmpty(spawnPointForPreviousRoom))
        {
            Debug.LogWarning($"RoomTransition: No opposite spawn point defined for '{correspondingSpawnPointName}'. Defaulting to 'PlayerSpawn'.");
            spawnPointForPreviousRoom = "PlayerSpawn"; // Fallback if a specific opposite isn't defined
        }

        GameManager.Instance.GoBackToLastRoom(spawnPointForPreviousRoom); // Pass the CORRECT spawn point for the previous room
    }
    else
    {
    
        // This door is a forward-moving door
                Debug.Log($"Door '{gameObject.name}' (associated with '{correspondingSpawnPointName}') is acting as a FORWARD door.");
        string nextRoomSceneName = GameManager.Instance.GetNextRandomRoom();
        if (!string.IsNullOrEmpty(nextRoomSceneName))
        {
            GameManager.Instance.LoadAndPlacePlayer(nextRoomSceneName, targetSpawnPointNameInNextRoom, correspondingSpawnPointName);
        }
        else
        {
            Debug.LogError("RoomTransition: GetNextRandomRoom returned an empty scene name. Cannot transition.");
        }
    }
}
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   

    }
}
