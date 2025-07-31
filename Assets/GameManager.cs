using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // Make sure this is included for Stack and Dictionary

public class GameManager : MonoBehaviour

{
   
    public GameObject gameOverPanel;
 
    public static GameManager Instance;


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
        if (gameOverPanel != null) 
        {
            gameOverPanel.SetActive(true); // Show the game over panel
        }
        Time.timeScale = 0f; 
        Debug.Log("Game Over! Time scaled to 0.");
    }









    public void RestartGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        Debug.Log("Restart game button pressed");
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene")   ;  
    }
}
