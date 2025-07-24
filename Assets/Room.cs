
using UnityEngine; // Required for GameObject and Vector2

// System.Serializable makes this class visible in the Inspector for debugging purposes.
[System.Serializable]
public class Room
{
    public RoomType roomType;
    public GameObject roomPrefab;
    public bool visited;
    public RoomDoors openDoors;

    [HideInInspector] public GameObject instantiatedRoomObject;

    public Vector2 worldPosition;

    // Constructor: Used to create new Room objects with initial properties.
    public Room(RoomType type = RoomType.Empty, GameObject prefab = null, RoomDoors doors = RoomDoors.None)
    {
        roomType = type;
        roomPrefab = prefab;
        visited = false;
        openDoors = doors;
        instantiatedRoomObject = null;
        worldPosition = Vector2.zero;
    }
}