using UnityEngine;

public class InventoryToggler : MonoBehaviour
{
    
    public GameObject inventoryPanel; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            }
        }
    }
}