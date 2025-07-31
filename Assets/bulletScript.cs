// BulletScript.cs
using UnityEngine;

public class BulletScript : MonoBehaviour
{
   
    public float damageToDeal = 0f; 


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyScript enemy = other.gameObject.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                // Use the damageToDeal value that was passed to this bullet
                enemy.TakeDamage(damageToDeal); 
                Debug.Log($"Bullet hit {other.name} for {damageToDeal} damage."); // Corrected log message
            }
            else
            {
                Debug.LogWarning($"Bullet hit object with tag 'Enemy' but no EnemyScript found on {other.name}!");
            }

            // Destroy the bullet after it hits an enemy
            Destroy(gameObject); 
        }
        // If the bullet hits anything else (like a wall, not an enemy), destroy it too
        else if (!other.CompareTag("Player")) // Make sure it doesn't destroy on hitting the player's own collider
        {
             Destroy(gameObject);
        }
    }
}