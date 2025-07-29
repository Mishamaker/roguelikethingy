using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    // Make sure these are assigned in the Inspector for each room prefab!
    [Header("Door References")]
    public GameObject doorNorth;
    public GameObject doorSouth;
    public GameObject doorEast;
    public GameObject doorWest;

    // This variable was missing, crucial for door logic!
    private RoomDoors currentRoomDoorsState; 

    private List<GameObject> activeEnemiesInRoom = new List<GameObject>();
    private bool areDoorsLocked = false;

    // Renamed parameter to follow C# naming conventions (camelCase)
    public void SetupDoors(Room roomData)
    {
        // Store the room's intended door configuration
        currentRoomDoorsState = roomData.roomDoors; 

        // Activate/deactivate door visuals based on roomData
        SetDoorActiveState(doorNorth, (roomData.roomDoors & RoomDoors.North) != 0);
        SetDoorActiveState(doorSouth, (roomData.roomDoors & RoomDoors.South) != 0);
        SetDoorActiveState(doorEast, (roomData.roomDoors & RoomDoors.East) != 0);
        SetDoorActiveState(doorWest, (roomData.roomDoors & RoomDoors.West) != 0);

        // When doors are initially set up, they should be unlocked
        UnlockDoorsVisuals();
        SetDoorColliders(false); // Colliders should be off to allow passage
    }

    // Helper to activate/deactivate the door GameObject itself
    private void SetDoorActiveState(GameObject doorObject, bool isActive)
    {
        if (doorObject != null)
        {
            doorObject.SetActive(isActive);
        }
    }

  private void SetDoorColliders(bool enable)
    {
        if (doorNorth != null && (currentRoomDoorsState & RoomDoors.North) != 0) 
        {
            
            Collider2D collider = doorNorth.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = enable;
            }
        }
        if (doorSouth != null && (currentRoomDoorsState & RoomDoors.South) != 0) 
        {
           
            Collider2D collider = doorSouth.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = enable;
            }
        }
        if (doorEast != null && (currentRoomDoorsState & RoomDoors.East) != 0) 
        {
          
            Collider2D collider = doorEast.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = enable;
            }
        }
        if (doorWest != null && (currentRoomDoorsState & RoomDoors.West) != 0) 
        {
            Collider2D collider = doorWest.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = enable;
            }
        }
    }

    private void UnlockDoorsVisuals()
    {
       
        Debug.Log($"[RoomController {gameObject.name}] Doors look unlocked."); // For debugging
    }

    private void LockDoorsVisuals() 
    {
      
        Debug.Log($"[RoomController {gameObject.name}] Doors look locked."); // For debugging
    }

    // Changed to public so DungeonManager can call it
    public void LockAllActiveDoors() // Renamed for consistency
    {
        if (areDoorsLocked)
        {
            return; // Already locked
        }
        areDoorsLocked = true;
        SetDoorColliders(true); // Enable colliders to block passage
        LockDoorsVisuals();     // Update visuals to locked state
        Debug.Log($"[RoomController {gameObject.name}] Doors locked."); // Debugging
    }

    // Changed to public and consistent naming
    public void UnlockAllActiveDoors() // Renamed for consistency
    {
        if (!areDoorsLocked) // Simplified condition
        {
            return; // Already unlocked
        }
        areDoorsLocked = false;
        SetDoorColliders(false); // Disable colliders to allow passage
        UnlockDoorsVisuals();    // Update visuals to unlocked state
        Debug.Log($"[RoomController {gameObject.name}] Doors unlocked."); // Debugging
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy != null && !activeEnemiesInRoom.Contains(enemy))
        {
            activeEnemiesInRoom.Add(enemy);
            Debug.Log($"[RoomController {gameObject.name}] Enemy registered: {enemy.name}. Total: {activeEnemiesInRoom.Count}"); // Added Debug.Log back
        }
    }

    public void EnemyDefeated(GameObject enemy)
    {
        if (enemy != null && activeEnemiesInRoom.Contains(enemy))
        {
            activeEnemiesInRoom.Remove(enemy);
            Debug.Log($"[RoomController {gameObject.name}] Enemy defeated: {enemy.name}. Remaining: {activeEnemiesInRoom.Count}");
            if (activeEnemiesInRoom.Count == 0)
            {
                UnlockAllActiveDoors(); // Calls the now correctly named public method
            }
        }
    }

    public bool HasEnemies()
    {
        return activeEnemiesInRoom.Count > 0;
    }

    void OnDisable()
    {
        activeEnemiesInRoom.Clear();
        areDoorsLocked = false; // Reset lock state
    }


}