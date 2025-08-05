using UnityEngine;

public class ItemPickup1 : MonoBehaviour
{
    public Item item;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // Get a reference to the SpriteRenderer component on this object
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Set the sprite to the one from the ScriptableObject
        if (item != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = item.icon;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // The rest of this is the same
            InventoryManager.Instance.AddItem(item);
            Destroy(gameObject);
            Debug.Log("Player collided and pickedup");
        }
        else
        {
            Debug.Log(" it did not collide with the player but something else");
        }
    }
}