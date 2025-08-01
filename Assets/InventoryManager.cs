using UnityEngine;
using System.Collections.Generic;

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
    private static InventoryManager m_Instance;
    public static InventoryManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<InventoryManager>();
            }

            if (m_Instance == null)
            {
                GameObject singletonObject = new GameObject();
                m_Instance = singletonObject.AddComponent<InventoryManager>();
                singletonObject.name = "InventoryManager (Singleton)";
            }
            return m_Instance;
        }
    }
    
    public event System.Action OnInventoryChanged;

    void Awake()
    {
        if (m_Instance != null && m_Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
        // DontDestroyOnLoad(gameObject);
    }


    [Header("Inventory Settings")]
    public int inventorySize =10;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();


    public bool AddItem(Item itemToAdd, int quantity = 1)
    {
        if (itemToAdd == null)
        {
            Debug.LogWarning("Attempted to add a null item to inventory!");
            return false;
        }
        
        bool itemAdded = false;

        if (itemToAdd.isStackable)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.item == itemToAdd)
                {
                    slot.quantity += quantity;
                    itemAdded = true;
                    break;
                }
            }
        }
        
        if (!itemAdded && inventorySlots.Count < inventorySize)
        {
            inventorySlots.Add(new InventorySlot(itemToAdd, quantity));
            itemAdded = true;
        }

        if (itemAdded)
        {
            OnInventoryChanged?.Invoke();
            return true;
        }
        else
        {
            Debug.LogWarning($"Inventory is full! Could not add {itemToAdd.itemName}.");
            return false;
        }
    }

    public bool RemoveItem(Item itemToRemove, int quantity = 1)
    {
        if (itemToRemove == null)
        {
            Debug.LogWarning("Attempted to remove a null item from inventory!");
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

                if (targetSlot.quantity <= 0)
                {
                    inventorySlots.Remove(targetSlot);
                }

                OnInventoryChanged?.Invoke();
                return true;
            }
            else
            {
                Debug.LogWarning($"Not enough {itemToRemove.itemName} to remove {quantity}. Has: {targetSlot.quantity}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning($"{itemToRemove.itemName} not found in inventory.");
            return false;
        }
    }

    public bool HasItem(Item itemToCheck, int quantityNeeded = 1)
    {
        if (itemToCheck == null) return false;

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
            if (slot.item == itemToCheck)
            {
                return slot.quantity;
            }
        }
        return 0;
    }

    public void UseBuffItem(BuffItem buffItem)
    {
        if (buffItem == null)
        {
            Debug.LogWarning("Attempted to use a null buff item from inventory!");
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
            Debug.LogError("PlayerStats not found! Cannot apply buff from item.");
        }
    }
}