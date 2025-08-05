using UnityEngine;

public class ItemDropHandler : MonoBehaviour
{
    // The prefab for the physical item that will be spawned in the world.
    public GameObject itemDropPrefab;

    // The list of possible items this enemy can drop.
    public Item[] possibleDrops;

    // The chance (0 to 1) that an item will drop at all.
    [Range(0f, 1f)]
    public float dropChance = 0.5f; // 50% chance by default

    // This method should be called from the enemy's Die() or OnDeath() function.
    public void DropItem()
    {
        // Step 1: Check if an item should drop at all.
        if (UnityEngine.Random.value <= dropChance)
        {
            // Step 2: Randomly select an item from the array.
            int randomIndex = UnityEngine.Random.Range(0, possibleDrops.Length);
            Item itemToDrop = possibleDrops[randomIndex];

            // Step 3: Instantiate the physical item at the enemy's position.
            GameObject droppedItem = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);

           
            ItemPickup1 itemPickupScript = droppedItem.GetComponent<ItemPickup1>();
            if (itemPickupScript != null)
            {
              
                itemPickupScript.item = itemToDrop;
            }
            else
            {
                
                Debug.LogError("The itemDropPrefab is missing the ItemPickup1 script!");
            }
        }
        else
        {
            Debug.Log("The player got unlucky and nothing dropped");
        }
    }
}