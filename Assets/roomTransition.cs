using UnityEngine;
using UnityEngine.SceneManagement;
public class roomTransition : MonoBehaviour
{
    public string targetSpawnPointNameInNextRoom;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {   string nextRoomSceneName = GameManager.Instance.GetNextRoom();
            GameManager.Instance.LoadAndPlacePlayer(nextRoomSceneName, targetSpawnPointNameInNextRoom);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   

    }
}
