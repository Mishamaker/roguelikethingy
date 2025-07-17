using UnityEngine;

public class playerSpawn : MonoBehaviour
{
    public Transform spawnpoint;
    public GameObject prefabplayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player=Instantiate(prefabplayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
