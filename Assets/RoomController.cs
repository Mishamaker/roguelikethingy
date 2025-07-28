using UnityEngine;

public class RoomController : MonoBehaviour
{
    public GameObject doorNorth;
    public GameObject doorSouth;
    public GameObject doorEast;
    public GameObject doorWest;

    public void SetupDoors(Room roomData){
        if (doorNorth!=null)doorNorth.SetActive((roomData.roomDoors&RoomDoors.North)!=0);
        if (doorSouth!=null)doorSouth.SetActive((roomData.roomDoors&RoomDoors.South)!=0);
        if (doorEast!=null)doorEast.SetActive((roomData.roomDoors&RoomDoors.East)!=0);
        if (doorWest!=null)doorWest.SetActive((roomData.roomDoors&RoomDoors.West)!=0);
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
