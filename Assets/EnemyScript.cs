using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float zombieHealth = 100f;
    public Transform playerTransform;
    public Transform zombieTransform;
    public float zombieSpeed = 5f;
    public Rigidbody2D zombieRigidBody;
    public float enemyDamage = 2;   

    public float retryFindPlayerInterval = .5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    if (playerTransform == null) 
    {
        InvokeRepeating("FindPlayerAndAssignPlayer", retryFindPlayerInterval, retryFindPlayerInterval);
    }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 zombieDirection = ((Vector2)playerTransform.position - (Vector2)zombieTransform.position).normalized;
        zombieRigidBody.linearVelocity = zombieDirection * zombieSpeed;
    }
    void FindPlayerAndAssignPlayer()
    {
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (playerGameObject != null)
        {
            playerTransform = playerGameObject.transform; // Assign it!
            Debug.Log("EnemyScript: Player found!");
            CancelInvoke("FindPlayerAndAssignPlayer"); // Stop the repeating calls
        }
        else
        {
            Debug.LogWarning("EnemyScript: Player not found yet. Retrying...");
        }

    }
    public void TakeDamage(float damage)
    {

        zombieHealth = zombieHealth - damage;
        if (zombieHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
