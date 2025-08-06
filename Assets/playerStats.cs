// PlayerStats.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class PlayerStats : MonoBehaviour
{
    public event Action OnStatsChanged;
    private GameManager GameManager;
    [Header("Core Stats")]
    public float maxHealth = 100f; // Maximum health the player can have
    public float currentHealth;    
    public float moveSpeed = 25f;   // Player's base movement speed
    public float baseDamage = 40f;
    [Header("Ability States")]
    public bool isDualWielding = false;     // Flag for temporary dual-wielding ability
    public bool isInvincible = false;       // Flag for temporary invincibility
   

    void Awake()
    {
        // Set current health to max health at the start of the game/level.
        currentHealth = maxHealth;
    }

    // Update() is called once per frame. (Currently empty, but kept for potential future use)
    void Update()
    {
       
    }
    public void ResetStats()
    {
        maxHealth = 100;
        currentHealth = 100;
        moveSpeed = 25;
        baseDamage = 40;
        Debug.Log("player stats reverted");
        OnStatsChanged?.Invoke();   
    }


    public void ApplyBuff(BuffItem buff)
    {

        if (buff == null)
        {
            Debug.LogWarning("Attempted to apply a null buff!");
            return;
        }

        Debug.Log($"Applying buff: {buff.name} (Type: {buff.buffEffectType}, Amount: {buff.buffAmount}, Duration: {buff.buffDuration})");

        // A switch statement to handle different types of buffs.
        switch (buff.buffEffectType)
        {
            // --- Instant Buffs ---
            case BuffEffectType.HealthBoost:
                currentHealth += buff.buffAmount; // Increase current health
                currentHealth = Mathf.Min(currentHealth, maxHealth); // Cap health at maxHealth
                Debug.Log($"Health increased to: {currentHealth}");
                break;

            // --- Timed Buffs (handled by Coroutines) ---
            case BuffEffectType.SpeedBoost:
                StartCoroutine(ApplyTimedSpeedBoost(buff.buffAmount, buff.buffDuration));
                break;

            case BuffEffectType.DamageBoost:
                StartCoroutine(ApplyTimedDamageBoost(buff.buffAmount, buff.buffDuration));
                break;

            case BuffEffectType.DualWielding:
                StartCoroutine(ApplyTimedDualWielding(buff.buffDuration));
                break;

            case BuffEffectType.TemporaryInvincibility:
                StartCoroutine(ApplyTimedInvincibility(buff.buffDuration));
                break;

            // --- Permanent Buffs (no Coroutine needed, direct modification) ---
            case BuffEffectType.PermanentMaxHealthIncrease:
                maxHealth += buff.buffAmount;   // Permanently increase max health
                currentHealth += buff.buffAmount; // Also increase current health by the same amount
                Debug.Log($"Permanent Max Health increased to: {maxHealth}. Current Health: {currentHealth}");
                break;

            case BuffEffectType.PermanentMoveSpeedIncrease:
                moveSpeed += buff.buffAmount;   // Permanently increase base movement speed
                Debug.Log($"Permanent Move Speed increased to: {moveSpeed}");
                break;

            case BuffEffectType.PermanentDamageIncrease:
                baseDamage += buff.buffAmount;   // Permanently increase base damage
                Debug.Log($"Permanent Base Damage increased to: {baseDamage}");
                break;



            // --- Default case for unhandled buff types ---
            default:
                Debug.LogWarning($"BuffEffectType {buff.buffEffectType} not handled in PlayerStats.ApplyBuff!");
                break;
        }
        OnStatsChanged?.Invoke();

        // TODO: (Optional) Update UI to show buff icon/timer for temporary buffs, or character sheet for permanent ones.
    }

    // --- Coroutines for Timed Buffs ---

    IEnumerator ApplyTimedSpeedBoost(float amount, float duration)
    {
        moveSpeed += amount;
        Debug.Log($"Speed Boost Applied! New speed: {moveSpeed}");
        OnStatsChanged?.Invoke();
        yield return new WaitForSeconds(duration); // Pause execution for the buff duration
        moveSpeed -= amount; // Revert speed after duration
        OnStatsChanged?.Invoke();
        Debug.Log($"Speed Boost Worn Off. New speed: {moveSpeed}");

    }

    IEnumerator ApplyTimedDamageBoost(float amount, float duration)
    {
        baseDamage += amount;
        OnStatsChanged?.Invoke();
        Debug.Log($"Damage Boost Applied! New damage: {baseDamage}");
        yield return new WaitForSeconds(duration);
        baseDamage -= amount;
        OnStatsChanged?.Invoke();
        Debug.Log($"Damage Boost Worn Off. New damage: {baseDamage}");
    }

    IEnumerator ApplyTimedDualWielding(float duration)
    {
        isDualWielding = true;
        Debug.Log("Dual Wielding activated!");
        OnStatsChanged?.Invoke();
        // TODO: Visual/animation changes for dual wielding
        yield return new WaitForSeconds(duration);
        OnStatsChanged?.Invoke();
        isDualWielding = false;
        Debug.Log("Dual Wielding deactivated!");
        // TODO: Revert visual/animation changes
    }

    IEnumerator ApplyTimedInvincibility(float duration)
    {
        isInvincible = true;
        Debug.Log("Invincibility activated!");
        OnStatsChanged?.Invoke();
        // TODO: Visual feedback (aura, flashing, particle effect)
        yield return new WaitForSeconds(duration);
        OnStatsChanged?.Invoke();
        isInvincible = false;
        Debug.Log("Invincibility deactivated!");
        // TODO: Remove visual feedback
    }

   
    public void TakeDamage(float damage)
    {
        // If player is invincible, take no damage.
        if (isInvincible)
        {
            Debug.Log("Player is invincible, no damage taken.");
            return;
        }

        currentHealth -= damage; // Deduct damage from current health
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");
        OnStatsChanged?.Invoke();

        // Check if health has dropped  to or below zero.
        if (currentHealth <= 0)
        {
            Die(); // Call the Die method
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
        
        GameManager.Instance.GameOver(); 
    }
}