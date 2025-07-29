using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    // Back to GameObject references
    [Header("Door References")]
    public GameObject doorNorth; // Assign the GameObject (with roomTransition & Collider2D)
    public GameObject doorSouth;
    public GameObject doorEast;
    public GameObject doorWest;

    private RoomDoors currentRoomDoorsState; 

    private List<GameObject> activeEnemiesInRoom = new List<GameObject>();
    private bool areDoorsLocked = false;

    public void SetupDoors(Room roomData)
    {
        Debug.Log($"[RoomController {gameObject.name}] SetupDoors called. RoomData: {roomData.roomDoors}");
        currentRoomDoorsState = roomData.roomDoors; 

        // Activate/deactivate door GameObjects (roomTransition will handle its own setup)
        SetDoorGameObjectActiveState(doorNorth, (roomData.roomDoors & RoomDoors.North) != 0);
        SetDoorGameObjectActiveState(doorSouth, (roomData.roomDoors & RoomDoors.South) != 0);
        SetDoorGameObjectActiveState(doorEast, (roomData.roomDoors & RoomDoors.East) != 0);
        SetDoorGameObjectActiveState(doorWest, (roomData.roomDoors & RoomDoors.West) != 0);

        // When rooms are initially set up, doors should be UNLOCKED (teleport active)
        SetAllActiveDoorsLocked(false); 
        Debug.Log($"[RoomController {gameObject.name}] Doors initially set to UNLOCKED during SetupDoors.");
    }

    // Helper to activate/deactivate the door GameObject itself
    private void SetDoorGameObjectActiveState(GameObject doorObject, bool isActive)
    {
        if (doorObject != null)
        {
            doorObject.SetActive(isActive);
            Debug.Log($"[RoomController {gameObject.name}] Door {doorObject.name} SetActive: {isActive}");
        } else {
            Debug.LogWarning($"[RoomController {gameObject.name}] Attempted to set active state for a null door object.");
        }
    }

    // This method now directly enables/disables the Collider2D on the door GameObjects
    private void SetAllActiveDoorsLocked(bool isLocked)
    {
        Debug.Log($"[RoomController {gameObject.name}] SetAllActiveDoorsLocked called with isLocked: {isLocked}");

        ProcessDoorCollider(doorNorth, RoomDoors.North, isLocked);
        ProcessDoorCollider(doorSouth, RoomDoors.South, isLocked);
        ProcessDoorCollider(doorEast, RoomDoors.East, isLocked);
        ProcessDoorCollider(doorWest, RoomDoors.West, isLocked);
    }

    private void ProcessDoorCollider(GameObject doorObject, RoomDoors doorDirection, bool isLocked)
    {
        // Only try to set state for doors that are supposed to exist in this room
        if (doorObject != null && (currentRoomDoorsState & doorDirection) != 0) 
        {
            Collider2D collider = doorObject.GetComponent<Collider2D>();
            if (collider != null) 
            {
                collider.enabled = !isLocked; // This is the core logic: if isLocked is TRUE, collider.enabled becomes FALSE
                Debug.Log($"[RoomController {gameObject.name}] Door {doorObject.name} collider.enabled set to: {collider.enabled} (based on isLocked: {isLocked})");
            }
            else
            {
                Debug.LogError($"[RoomController {gameObject.name}] Door {doorObject.name} is missing a Collider2D component!");
            }
        }
        else if (doorObject == null)
        {
            Debug.LogWarning($"[RoomController {gameObject.name}] Attempted to process a null door object for {doorDirection}. Is it assigned in Inspector?");
        }
        else // This door doesn't exist for this room type, so we don't need to process its collider
        {
             // Uncomment if you want to see logs for doors that are active in prefab but not in room data
             // Debug.Log($"[RoomController {gameObject.name}] Door {doorObject.name} for {doorDirection} not relevant for this room's doors state: {currentRoomDoorsState}");
        }
    }

    // These methods are for visual effects if you want them, now triggered by RoomController
    private void UnlockDoorsVisuals(){
        Debug.Log($"[RoomController {gameObject.name}] Doors visuals unlocked."); 
        // Add your visual changes here if you want room-wide effects
    }

    private void LockDoorsVisuals() { 
        Debug.Log($"[RoomController {gameObject.name}] Doors visuals locked."); 
        // Add your visual changes here if you want room-wide effects
    }

    public void LockAllActiveDoors() 
    {
        Debug.Log($"[RoomController {gameObject.name}] LockAllActiveDoors called. Current areDoorsLocked: {areDoorsLocked}");
        if (areDoorsLocked)
        {
            Debug.Log($"[RoomController {gameObject.name}] Doors already locked, returning.");
            return; 
        }
        areDoorsLocked = true;
        SetAllActiveDoorsLocked(true); // Disable colliders (lock teleport)
        LockDoorsVisuals(); // Update visuals
        Debug.Log($"[RoomController {gameObject.name}] Doors are now set to LOCKED.");
    }

    public void UnlockAllActiveDoors() 
    {
        Debug.Log($"[RoomController {gameObject.name}] UnlockAllActiveDoors called. Current areDoorsLocked: {areDoorsLocked}");
        if (!areDoorsLocked) 
        {
            Debug.Log($"[RoomController {gameObject.name}] Doors already unlocked, returning.");
            return; 
        }
        areDoorsLocked = false;
        SetAllActiveDoorsLocked(false); // Enable colliders (unlock teleport)
        UnlockDoorsVisuals(); // Update visuals
        Debug.Log($"[RoomController {gameObject.name}] Doors are now set to UNLOCKED.");
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy != null && !activeEnemiesInRoom.Contains(enemy))
        {
            activeEnemiesInRoom.Add(enemy);
            Debug.Log($"[RoomController {gameObject.name}] Enemy registered: {enemy.name}. Total: {activeEnemiesInRoom.Count}");
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
                Debug.Log($"[RoomController {gameObject.name}] All enemies defeated, calling UnlockAllActiveDoors.");
                UnlockAllActiveDoors(); 
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
        areDoorsLocked = false; 
        Debug.Log($"[RoomController {gameObject.name}] OnDisable called. Enemy list cleared, doors unlocked state reset.");
    }
}