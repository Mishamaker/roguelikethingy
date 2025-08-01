using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class InventoryUI : MonoBehaviour
{

    [Header("UI References")]
    public GameObject inventorySlotUIPrefab;
    public Transform slotsParent;
    public GameObject Inventory;
    private InventoryManager inventoryManager;
    private List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();
    void Awake()
    {
        inventoryManager = InventoryManager.Instance;
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
            Debug.Log("No inventorymanager");
            return;
        }
        inventoryManager.OnInventoryChanged += RefreshInventoryUI;
        InitializeUISlots();
    }
    
    void OnEnable()
    {
        RefreshInventoryUI();
    }
    void OnDisable()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged -= RefreshInventoryUI;
        }
    }
    
    void InitializeUISlots()
    {
        if (inventorySlotUIPrefab == null)
        {
            Debug.Log("No prefab for inventory slot ui");
            return;
        }
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }
        uiSlots.Clear();

        for (int i = 0; i < inventoryManager.inventorySize; i++)
        {
            GameObject slotGO = Instantiate(inventorySlotUIPrefab, slotsParent);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            if (slotUI == null)
            {
                Debug.Log("No slotUi exists");
                return;
            }
            uiSlots.Add(slotUI);
            slotUI.name = "$InventorySlot_{i}";
        }
        RefreshInventoryUI();
    }
    public void RefreshInventoryUI()
    {
        for (int i = 0; i < uiSlots.Count; i++)
        {
            if (i < inventoryManager.inventorySlots.Count)
            {
                uiSlots[i].SetSlot(inventoryManager.inventorySlots[i]);

            }
            else
            {
                uiSlots[i].SetSlot(null);

            }
        }
    }
}