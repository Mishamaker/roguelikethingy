using UnityEngine;
using System.Collections.Generic;
using System;

public class RoomController : MonoBehaviour
{
    [Header("Door References")]
    public GameObject doorNorth;
    public GameObject doorSouth;
    public GameObject doorEast;
    public GameObject doorWest;

    private RoomDoors currentRoomDoorsState; 

    private List<GameObject> activeEnemiesInRoom = new List<GameObject>();
    private bool areDoorsLocked = false;
    public void Awake()
    {
    
    }
    public void SetupDoors(Room roomData)
    {
        currentRoomDoorsState = roomData.roomDoors; 

        SetDoorGameObjectActiveState(doorNorth, (roomData.roomDoors & RoomDoors.North) != 0);
        SetDoorGameObjectActiveState(doorSouth, (roomData.roomDoors & RoomDoors.South) != 0);
        SetDoorGameObjectActiveState(doorEast, (roomData.roomDoors & RoomDoors.East) != 0);
        SetDoorGameObjectActiveState(doorWest, (roomData.roomDoors & RoomDoors.West) != 0);

        SetAllActiveDoorsLocked(false); 
    }

    private void SetDoorGameObjectActiveState(GameObject doorObject, bool isActive)
    {
        if (doorObject != null)
        {
            doorObject.SetActive(isActive);
        }
    }

    private void SetAllActiveDoorsLocked(bool isLocked)
    {
        ProcessDoorCollider(doorNorth, RoomDoors.North, isLocked);
        ProcessDoorCollider(doorSouth, RoomDoors.South, isLocked);
        ProcessDoorCollider(doorEast, RoomDoors.East, isLocked);
        ProcessDoorCollider(doorWest, RoomDoors.West, isLocked);
    }

    private void ProcessDoorCollider(GameObject doorObject, RoomDoors doorDirection, bool isLocked)
    {
        if (doorObject != null && (currentRoomDoorsState & doorDirection) != 0) 
        {
            Collider2D collider = doorObject.GetComponent<Collider2D>();
            if (collider != null) 
            {
                collider.enabled = !isLocked;
            }
        }
    }

    private void UnlockDoorsVisuals(){

    }

    private void LockDoorsVisuals() { 
    }

    public void LockAllActiveDoors() 
    {
        if (areDoorsLocked)
        {
            return; 
        }
        areDoorsLocked = true;
        SetAllActiveDoorsLocked(true);
        LockDoorsVisuals();
    }

    public void UnlockAllActiveDoors() 
    {
        if (!areDoorsLocked) 
        {
            return; 
        }
        areDoorsLocked = false;
        SetAllActiveDoorsLocked(false);
        UnlockDoorsVisuals();
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy != null && !activeEnemiesInRoom.Contains(enemy))
        {
            activeEnemiesInRoom.Add(enemy);
        }
    }

    public void EnemyDefeated(GameObject enemy)
    {
        if (enemy != null && activeEnemiesInRoom.Contains(enemy))
        {
            activeEnemiesInRoom.Remove(enemy);
            if (activeEnemiesInRoom.Count == 0)
            {
                UnlockAllActiveDoors();
                
                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.SetBattleMusic(false); 
                }
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
    }

    public void CheckRoomForEnemiesAndLockDoors(GameObject roomObject)
    {
        int enemiesFound = 0;
        foreach (Transform child in roomObject.transform)
        {
            if (child.CompareTag("Enemy"))
            {
                RegisterEnemy(child.gameObject);
                enemiesFound++;
            }
        }

        if (enemiesFound > 0)
        {
            if (MusicManager.Instance != null) 
            {
                MusicManager.Instance.SetBattleMusic(true);
            }
            LockAllActiveDoors();
        }
        else
        {
            if (MusicManager.Instance != null) 
            {
                MusicManager.Instance.SetBattleMusic(false);
            }
            UnlockAllActiveDoors();
        }
    }
}