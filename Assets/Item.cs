
using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon = null; // Visual representation of the item in UI
    public bool isStackable = false; // Can multiple of this item occupy one inventory slot?
    public int maxStackSize = 1; // How many can stack in one slot? (if isStackable is true)

    [TextArea(3, 10)] // Makes the string field a multi-line text area in the Inspector
    public string description = "A generic item.";

 
    public virtual void Use()
    {
        Debug.Log($"Using {itemName}");
        
    }
}