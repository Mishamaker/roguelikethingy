using UnityEngine;
using System.Collections; // Required for Coroutines

public class playerScript : MonoBehaviour
{
    // Reference to the PlayerStats script (this will hold health, speed, damage, etc.)
    private PlayerStats playerStats; 

    public Rigidbody2D myRigidbody;
    // public GameObject player; // Redundant, 'this.gameObject' refers to the player

    // Combat related (some values might come from PlayerStats directly if buffable)
    public GameObject bulletprefab;
    public float projectilespeed = 3f; // Projectile speed can still be here or be a PlayerStats property
    private bool isshooting = false;
    public float gunCoolDown;
    public int shootMouseButton = 0; // Left mouse button
    public float delay = 5f; // Bullet destroy delay

    // Dash related (dashStrength could also be a PlayerStats property)
    public float dashStrength; 
    public float dashDuration = 0.2f;
    private bool isdashing = false;
    public float dashCooldown = 1.0f;
    private float nextDashTime = 0f;

    // Damage tick rate (for continuous damage from enemies)
    public float damageTickRate = .1f;
    public float nextDamageTime = 0;

    void Awake()
    {
        // Get references to components on this same GameObject
        myRigidbody = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>(); // Get the PlayerStats component!

        // Error checking for missing components
        if (myRigidbody == null)
        {
            Debug.LogError("Rigidbody2D not found on playerScript's GameObject!");
        }
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found on playerScript's GameObject! Make sure it's attached.");
        }

        // Only use DontDestroyOnLoad if your player truly persists across ALL scenes.
        // Be careful with this, as it can lead to duplicate players if not managed well.
        DontDestroyOnLoad(this.gameObject);
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (!isdashing) 
        {
            
            Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
            myRigidbody.linearVelocity = movement * playerStats.moveSpeed;
        }

        if (Input.GetKey(KeyCode.Space) && !isdashing && Time.time >= nextDashTime)
        {
            StartCoroutine(Dasher());
            nextDashTime = Time.time + dashCooldown;
        }

        // Shooting input
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mouseWorldPos - (Vector2)transform.position).normalized;

        if (Input.GetMouseButton(shootMouseButton) && !isshooting)
        {
           
            if (shootDirection.magnitude < 0.1f)
            {
                shootDirection = Vector2.right; // Default direction if mouse is on player
            }
            StartCoroutine(BulletShooter(shootDirection));
        }
    }

    
    IEnumerator BulletShooter(Vector2 bulletDirection)
    {
        isshooting = true;
        GameObject bullet = Instantiate(bulletprefab, transform.position, transform.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = bulletDirection * projectilespeed;
        BulletScript bulletScript = bullet.GetComponent<BulletScript>(); 
        if (playerStats.isDualWielding==true){
            
        }

       if (bulletScript != null)
        {
            bulletScript.damageToDeal = playerStats.baseDamage;
            Debug.Log($"[playerScript] Set bullet's damageToDeal to: {bulletScript.damageToDeal}");

        }
        Destroy(bullet, delay);
        yield return new WaitForSeconds(gunCoolDown);
        isshooting = false;
    }

    
    IEnumerator Dasher()
    {
        isdashing = true;
        // Apply dash force based on current movement direction
        Vector2 dashVelocity = myRigidbody.linearVelocity.normalized * dashStrength;
        // If player is stationary, dash in a default direction (e.g., forward or right)
        if (dashVelocity.magnitude < 0.1f)
        {
             // You could get a default direction from input or player facing
             dashVelocity = transform.right * dashStrength; // Example: dash to the right
        }
        myRigidbody.linearVelocity = dashVelocity;

        yield return new WaitForSeconds(dashDuration);

        isdashing = false;
      
        myRigidbody.linearVelocity = Vector2.zero;
    }




    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && Time.time >= nextDamageTime)
        {
            EnemyScript enemyScript = collision.gameObject.GetComponent<EnemyScript>();

            if (enemyScript != null)
            {

                playerStats.TakeDamage(enemyScript.enemyDamage);
            }
            else
            {
                Debug.LogWarning("EnemyScript not found on colliding object with tag 'Enemy'!");
            }

            nextDamageTime = Time.time + damageTickRate;
            Debug.Log("Player got hit.");
        }
       void OnCollisionStay2D(Collision2D collision)
{
    // Handle regular enemy damage
    if (collision.gameObject.CompareTag("Enemy") && Time.time >= nextDamageTime)
    {
        EnemyScript enemyScript = collision.gameObject.GetComponent<EnemyScript>();
        if (enemyScript != null)
        {
            playerStats.TakeDamage(enemyScript.enemyDamage);
            Debug.Log("Player got hit by enemy.");
        }
        else
        {
            Debug.LogWarning("EnemyScript not found on colliding object with tag 'Enemy'!");
        }
        nextDamageTime = Time.time + damageTickRate;
    }
    
    // Handle boss damage
    if (collision.gameObject.CompareTag("Boss") && Time.time >= nextDamageTime)
    {
      
        BossAI bossScript = collision.gameObject.GetComponent<BossAI>();
        
        
        if (bossScript != null)
        {
          
            playerStats.TakeDamage(bossScript.bossDamage); 
            Debug.Log("Player got hit by boss.");
        }
        
       
        nextDamageTime = Time.time + damageTickRate;
    }
}
    }
}