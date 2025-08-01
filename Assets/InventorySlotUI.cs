using UnityEngine;
 using UnityEngine.UI; 
using TMPro;
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button button;
    private InventoryManager inventoryManager;
    private InventorySlot currentInventorySlot;
    void Awake()
    {
        if (iconImage == null) iconImage = transform.Find("Icon").GetComponent<Image>();
        if (quantityText == null) quantityText = transform.Find("QuantityText").GetComponent<TextMeshProUGUI>();
        if (button == null) button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnSlotClicked);

        }
        else
        {
            Debug.Log("No button existing");
        }

    }
    public void SetSlot(InventorySlot slot)
    {
        currentInventorySlot = slot;
        if (slot != null && slot.item != null)
        {
            iconImage.sprite = slot.item.icon;
            iconImage.enabled = true;
            quantityText.text = slot.quantity.ToString();
            quantityText.enabled = true;
            button.interactable = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            quantityText.text = "";
            quantityText.enabled = false;
            button.interactable = false;
        }
    }
 // InventorySlotUI.cs (only showing the OnSlotClicked method)
private void OnSlotClicked()
{
    // Make sure we have an item to work with
    if (currentInventorySlot != null && currentInventorySlot.item != null)
    {
      
        BuffItem buffItem = currentInventorySlot.item as BuffItem; 
        
       
        if (buffItem != null) // This condition is ONLY true if the item was a BuffItem
        {
           
            InventoryManager.Instance.UseBuffItem(buffItem);
        }
        else
        {
            
        }
    }
}
    }

