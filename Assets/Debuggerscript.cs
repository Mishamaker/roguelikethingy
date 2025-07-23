using UnityEngine;
using UnityEngine.SceneManagement; // Needed for SceneManager

public class RoomActivityDebugger : MonoBehaviour
{
    // Awake is called when the script instance is being loaded.
    // This will run very early, when the GameObject is initialized.
    void Awake()
    {
        Debug.Log($"<color=lime>RoomActivityDebugger on '{gameObject.name}' in scene '{gameObject.scene.name}': AWAKE called. IsActive: {gameObject.activeSelf}</color>");
        // Immediately log if it's not active even in Awake
        if (!gameObject.activeSelf)
        {
            Debug.LogError($"<color=red>RoomActivityDebugger WARNING: '{gameObject.name}' in '{gameObject.scene.name}' IS INACTIVE in Awake!</color>");
        }
    }

    // OnEnable is called when the object becomes enabled and active.
    void OnEnable()
    {
        Debug.Log($"<color=blue>RoomActivityDebugger on '{gameObject.name}' in scene '{gameObject.scene.name}': ONENABLE called. IsActive: {gameObject.activeSelf}</color>");
    }

    // Start is called on the frame when a script is first enabled just before any Update methods are called.
    void Start()
    {
        Debug.Log($"<color=green>RoomActivityDebugger on '{gameObject.name}' in scene '{gameObject.scene.name}': START called. IsActive: {gameObject.activeSelf}</color>");
    }

    // OnDisable is called when the behaviour becomes disabled or inactive.
    void OnDisable()
    {
        Debug.Log($"<color=red>RoomActivityDebugger on '{gameObject.name}' in scene '{gameObject.scene.name}': ONDISABLE called. IsActive: {gameObject.activeSelf}</color>");
    }

    // OnDestroy is called when the MonoBehaviour will be destroyed.
    void OnDestroy()
    {
        Debug.Log($"<color=orange>RoomActivityDebugger on '{gameObject.name}' in scene '{gameObject.scene.name}': ONDESTROY called.</color>");
    }

 }
