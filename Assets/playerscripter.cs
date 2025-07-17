using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
public class Playerscripter : MonoBehaviour
{
    public Transform playerTransform; // Target to follow
    public float cameraZOffset;       // Stores the fixed Z position for the camera

   
    private static Playerscripter instance; 

    void Awake()
    {

        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initialize Z offset from the camera's initial position
        cameraZOffset = transform.position.z;
    }

    void OnDestroy() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find the player in the newly loaded scene.
        // This is crucial because the player might have been repositioned by GameManager.OnSceneLoaded.
        FindPlayerTarget(); 
    }

    
    void FindPlayerTarget()
    {
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");

        if (playerGameObject != null)
        {
            playerTransform = playerGameObject.transform;
            Debug.Log("CameraFollow: Found player target: " + playerGameObject.name);
        }
        else
        {
            playerTransform = null;
            Debug.LogWarning("CameraFollow: Player with tag 'Player' NOT found. Camera will not follow.");
        }
    }

    void Start() 
    {
        // Find the player when the game starts (initial scene)
        FindPlayerTarget();
    }

    void LateUpdate()
    {
 
        if (playerTransform == null)
        {
           FindPlayerTarget(); 
        }

        Vector3 playerPosition = playerTransform.position;
        // The camera only follows on X and Y, keeping its Z fixed
        transform.position = new Vector3(playerPosition.x, playerPosition.y, cameraZOffset);
    }
}