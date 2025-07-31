using UnityEngine;

public class EnemyScript : MonoBehaviour // Keeping the name EnemyScript for now
{
    public float zombieHealth = 100f; // Existing health
    public Transform playerTransform;
    public Transform zombieTransform; // Likely just transform, but kept for consistency
    public float zombieSpeed = 5f;
    public Rigidbody2D zombieRigidBody; // Assign this in Inspector
    public float enemyDamage = 2;       // Damage dealt by this enemy

    public float retryFindPlayerInterval = .5f;

    // NEW: Reference to the RoomController of the room this enemy belongs to
    private RoomController parentRoomController; 

    void Start()
    {
      
        if (playerTransform == null) 
        {
            InvokeRepeating("FindPlayerAndAssignPlayer", retryFindPlayerInterval, retryFindPlayerInterval);
        }
       

     
        parentRoomController = GetComponentInParent<RoomController>();
        if (parentRoomController != null)
        {
            parentRoomController.RegisterEnemy(gameObject);
            Debug.Log($"[EnemyScript] Registered with RoomController: {parentRoomController.gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"[EnemyScript] Enemy '{gameObject.name}' could not find a parent RoomController! " +
                             "Ensure enemies are children of a room with a RoomController prefab.");
        }

        // Initialize zombieTransform if it's null (it should usually be transform by default)
        if (zombieTransform == null)
        {
            zombieTransform = this.transform;
        }
        // Initialize Rigidbody2D if not assigned in Inspector (good practice)
        if (zombieRigidBody == null)
        {
            zombieRigidBody = GetComponent<Rigidbody2D>();
            if (zombieRigidBody == null)
            {
                Debug.LogError($"[EnemyScript] No Rigidbody2D found on {gameObject.name}. Movement might not work.");
            }
        }
    }

    void FixedUpdate()
    {
        // Only move if playerTransform is found and Rigidbody exists
        if (playerTransform != null && zombieRigidBody != null)
        {
            Vector2 zombieDirection = ((Vector2)playerTransform.position - (Vector2)zombieTransform.position).normalized;
            zombieRigidBody.linearVelocity = zombieDirection * zombieSpeed; // Changed to .velocity for 2D
        }
        else if (zombieRigidBody != null)
        {
            zombieRigidBody.linearVelocity = Vector2.zero; // Stop if no player
        }
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

    // Existing: Method for taking damage
    public void TakeDamage(float damage)
    {
        zombieHealth -= damage; // Use -= for subtraction
        Debug.Log($"[EnemyScript] {gameObject.name} took {damage} damage. Health: {zombieHealth}");

        if (zombieHealth <= 0)
        {
            Die(); 
        }
    }

    void Die()
    {
        if (parentRoomController != null)
        {
      
            parentRoomController.EnemyDefeated(gameObject); 
        }
        Debug.Log($"[EnemyScript] {gameObject.name} defeated!");
        Destroy(gameObject); // Destroy the enemy GameObject
    }



 
}