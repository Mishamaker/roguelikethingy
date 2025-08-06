using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;
    public float damage = 10f; 

    void Start()
    {
       
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
      
        if (collision.gameObject.CompareTag("Player"))
        {
            
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            
            if (playerStats != null)
            {
               
                playerStats.TakeDamage(damage);
            }
        }
        
        
        Destroy(gameObject);
    }
}