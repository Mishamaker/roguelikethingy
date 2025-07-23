using UnityEngine;

public class PlayerPersist : MonoBehaviour
{
    // A static reference to the single instance of PlayerPersist.
    // 'static' means this variable belongs to the class itself, not to any specific object.
    private static PlayerPersist instance;

    void Awake()
    {
        // Debug log to confirm Awake is called for this instance
        Debug.Log("PlayerPersist: Awake called for " + gameObject.name + ", Instance ID: " + GetInstanceID());

        // Check if an instance of PlayerPersist already exists
        if (instance == null)
        {

            instance = this;

            // Make this GameObject persistent across scene loads.
            // It will not be destroyed when a new scene is loaded.
            DontDestroyOnLoad(gameObject);
            Debug.Log("PlayerPersist: This is the persistent player. DontDestroyOnLoad called for " + gameObject.name);
        }
        else
        {
            // If an instance already exists, this is a duplicate player.
            // Destroy this duplicate GameObject immediately.
            Debug.LogWarning("PlayerPersist: Duplicate player found (" + gameObject.name + "). Destroying self.");
            Destroy(gameObject);
            // Optionally, return here to prevent any further code in this Awake
            // from running for the destroyed duplicate.
            return; 
        }
    }
}