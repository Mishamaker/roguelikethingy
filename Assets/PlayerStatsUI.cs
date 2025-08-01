using UnityEngine;
using TMPro;
using System; // Make sure you have this line as well.

public class PlayerStatsUI : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text SpeedText;
    public TMP_Text attackText;

    private PlayerStats playerStats;

    void Awake()
    {
       
        DungeonManager.OnPlayerSpawned += OnPlayerSpawnedHandler;
    }

    private void OnPlayerSpawnedHandler(GameObject playerObject)
    {
       
        playerStats = playerObject.GetComponent<PlayerStats>();
        
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats script not found on the spawned player!");
            return;
        }

        
        playerStats.OnStatsChanged += RefreshUI;
        
        
        RefreshUI();
    }

    private void RefreshUI()
    {
        
        healthText.text = $"{playerStats.currentHealth}/{playerStats.maxHealth}";
        attackText.text = $"{playerStats.baseDamage}";
        SpeedText.text = $"{playerStats.moveSpeed}";
    }

    void OnDestroy()
    {
        // It's very important to unsubscribe from both events when this object is destroyed.
        DungeonManager.OnPlayerSpawned -= OnPlayerSpawnedHandler;
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= RefreshUI;
        }
    }
}