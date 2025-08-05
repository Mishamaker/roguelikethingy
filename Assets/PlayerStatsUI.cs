using UnityEngine;
using TMPro;
using System;

public class PlayerStatsUI : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text SpeedText;
    public TMP_Text attackText;

    private PlayerStats playerStats;

    void Awake()
    {
        // First, check if the player has already spawned.
        if (DungeonManager.Instance.currentPlayerInstance != null)
        {
            // If the player exists, connect immediately.
            ConnectToPlayerStats(DungeonManager.Instance.currentPlayerInstance);
        }
        else
        {
            // If the player doesn't exist yet, subscribe to the event and wait.
            DungeonManager.OnPlayerSpawned += ConnectToPlayerStats;
        }
    }

    private void ConnectToPlayerStats(GameObject playerObject)
    {
        Debug.Log("Player Spawned Event Fired! Attempting to get PlayerStats.");

        playerStats = playerObject.GetComponent<PlayerStats>();
        
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats script not found on the spawned player!");
            return;
        }

        // Now that we have a reference, subscribe to the stat changes.
        playerStats.OnStatsChanged += RefreshUI;
        
        // Refresh the UI once to set the initial values.
        RefreshUI();
    }

    private void RefreshUI()
    {
        // Add a debug log here to confirm this method is running
        Debug.Log($"UI Refreshing! Health: {playerStats.currentHealth}, Attack: {playerStats.baseDamage}, Speed: {playerStats.moveSpeed}");

        healthText.text = $"{playerStats.currentHealth}/{playerStats.maxHealth}";
        attackText.text = $"{playerStats.baseDamage}";
        SpeedText.text = $"{playerStats.moveSpeed}";
    }

    void OnDestroy()
    {
        // Always unsubscribe to prevent errors.
        DungeonManager.OnPlayerSpawned -= ConnectToPlayerStats;
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= RefreshUI;
        }
    }
}