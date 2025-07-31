using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int quantity;
    public InventorySlot(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
        
    }
}
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);


        }
        else
        {
            Destroy(gameObject);
        }

    }
    [Header("inventory settings")]
    public int inventorySize = 20;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public bool AddItem(Item itemToAdd, int quantity = 1)
    {
        if (itemToAdd == null)
        {
            return false;
        }
        if (itemToAdd.isStackable)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.item == itemToAdd)
                {
                    slot.quantity += quantity;
                    Debug.Log($"Added {quantity} x {itemToAdd.itemName}. Total: {slot.quantity}");
                    return true;

                }
            }
        }
        if (inventorySlots.Count < inventorySize)
        {
            inventorySlots.Add(new InventorySlot(itemToAdd, quantity));
            return true;
        }
        else
        {
            Debug.Log("Inventoy is full cuhhh");
            return false;
        }
    }
    public bool RemoveItem(Item itemToRemove, int quantity = 1)
    {
        if (itemToRemove == null)
        {
            return false;
        }
        InventorySlot targetSlot = null;
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.item == itemToRemove)
            {
                targetSlot = slot;
                break;
            }
        }
        if (targetSlot != null)
        {
            if (targetSlot.quantity >= quantity)
            {
                targetSlot.quantity -= quantity;
                Debug.Log($"Removed {quantity} x {itemToRemove.itemName}. Remaining: {targetSlot.quantity}");
                if (targetSlot.quantity <= 0)
                {
                    inventorySlots.Remove(targetSlot);

                }
                return true;
            }
            else
            {
                return false;
            }
        }


        else
        {
            Debug.Log("Inventoy is full cuhhh");
            return false;
        }

    }
    public bool HasItem(Item itemToCheck, int quantityNeeded)
    {
        if (itemToCheck == null)
        {
            return false;
        }
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.item == itemToCheck && slot.quantity >= quantityNeeded)
            {
                return true;
            }

        }
        return false;
    }
    
    public int GetItemQuantity(Item itemToCheck)
    {
        if (itemToCheck == null) return 0;

        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.item == itemToCheck) // Finds the item
            {
                return slot.quantity; // Returns its quantity
            }
        }
        return 0; 
    }
    public void UseBuffItem(BuffItem buffItem)
    {
        if (buffItem == null)
        {
            return;
        }
        PlayerStats playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();

        }
        if (playerStats != null)
        {
            playerStats.ApplyBuff(buffItem);
            RemoveItem(buffItem, 1);

        }
        else
        {
            Debug.LogError("no playerstats ecist blud");
        }
     }
}