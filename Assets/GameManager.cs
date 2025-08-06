using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // Make sure this is included for Stack and Dictionary

public class GameManager : MonoBehaviour

{

    public GameObject gameOverPanel;
    PlayerStats playerStats;

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
       
    }

    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); 
        }
        Time.timeScale = 0f;
        Debug.Log("Game Over! Time scaled to 0.");
    }










    public void RetryButton()
    {

        if (DungeonManager.Instance != null)
        {
            Destroy(DungeonManager.Instance.gameObject);
        }

        if (MusicManager.Instance != null)
        {
            Destroy(MusicManager.Instance.gameObject);
        }


        GameObject oldPlayer = GameObject.Find("Player(Clone)");
        if (oldPlayer != null)
        {

            PlayerStats playerStatsScript = oldPlayer.GetComponent<PlayerStats>();

            if (playerStatsScript != null)
            {

                playerStatsScript.ResetStats();
            }

            Destroy(oldPlayer);
        }

        GameObject oldCamera = GameObject.Find("Main Camera");
        if (oldCamera != null && oldCamera.transform.parent == null)
        {
            Destroy(oldCamera);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);

        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
       
    }

}
