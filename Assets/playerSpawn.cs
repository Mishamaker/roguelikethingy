using UnityEngine;

public class playerSpawn : MonoBehaviour
{
    public Transform spawnpoint;
    public GameObject prefabplayer;
    void Awake()
    {
        GameObject player=Instantiate(prefabplayer);    
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
