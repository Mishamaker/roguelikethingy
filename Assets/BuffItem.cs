
using UnityEngine;


[CreateAssetMenu(fileName = "NewBuffItem", menuName = "Inventory/Buff Item")]
public class BuffItem : Item 
{
    [Header("Buff Properties")]
    public BuffEffectType buffEffectType = BuffEffectType.None; // The type of effect this buff applies
    public float buffAmount = 0f; // The magnitude of the buff (e.g., +10 health, +2 speed)
    public float buffDuration = 0f; // The duration of the buff in seconds (0 for instant/permanent)


    public override void Use()
    {
        base.Use();

        Debug.Log($"Applying {itemName} buff with type {buffEffectType} and amount {buffAmount} for {buffDuration} seconds.");

       
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.ApplyBuff(this); // 'this' refers to this specific BuffItem instance
        }
        else
        {
            Debug.LogError("PlayerStats component not found in the scene to apply buff!");
        }

    }
}