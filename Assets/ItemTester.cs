using UnityEngine;

public class ItemTester : MonoBehaviour
{
    // Make sure to drag your new item asset from the Project window into this slot in the Inspector
    public Item itemToAdd; 
    
    // An optional BuffItem to test using a different key
    public BuffItem buffItemToUse;

    void Update()
    {
        // Press the 'A' key to add the item
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (itemToAdd != null)
            {
                InventoryManager.Instance.AddItem(itemToAdd, 1);
            }
            else
            {
                Debug.LogWarning("Item to add is not set in the ItemTester script!");
            }
        }
        
        // You can also add a different key to test using a specific buff item
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (buffItemToUse != null)
            {
                InventoryManager.Instance.UseBuffItem(buffItemToUse);
            }
        }
    }
}