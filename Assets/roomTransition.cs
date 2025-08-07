using UnityEngine;
using UnityEngine.SceneManagement;
public class roomTransition : MonoBehaviour
{
    public string targetRoomType;
    public RoomDoors exitDirectionForThisDoor;
    public bool isLocked = false;
    private GameObject playerObject;
    public string targetSpawnPointNameInNextRoom;
    public string correspondingSpawnPointName;
    public DungeonManager dungeonManager;
        void Start()
    {
        dungeonManager = DungeonManager.Instance; 
    }


    void OnTriggerEnter2D(Collider2D other)

    {
        if (other.CompareTag("Player"))
        {
            dungeonManager.MovePlayer(exitDirectionForThisDoor);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {

    }

}
