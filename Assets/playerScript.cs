using UnityEngine;
using System.Collections;
public class playerScript : MonoBehaviour

{

    public Rigidbody2D myRigidbody;

    public GameObject player;
    public float projectilespeed = 3f;
    public float moveSpeed;
    public float dashStrength;
    public float dashDuration = 0.2f;
    private bool isdashing = false;
    public float dashCooldown = 1.0f; // Cooldown duration in seconds
    private float nextDashTime = 0f;

    public GameObject bulletprefab;
    private bool isshooting = false;
    public float gunCoolDown;
    private Vector2 shootDirection;
    public float delay = 5f;
    public int shootMouseButton = 0;
    public float playerHealth = 100f;
    public float damageTickRate = .1f;
    public float nextDamageTime = 0;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        myRigidbody = GetComponent<Rigidbody2D>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        // This gets info from user input
        if (isdashing == false) //this is making sure that the movement doesnt override the dash
        {
            Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
            myRigidbody.linearVelocity = movement * moveSpeed;
        }
        // This turns the info into user velocity

        if (Input.GetKey(KeyCode.Space))
            if (isdashing == false && Time.time >= nextDashTime)
            {
                StartCoroutine(Dasher());
                nextDashTime = Time.time + dashCooldown;
            }


        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mouseWorldPos - (Vector2)transform.position).normalized;
        if (Input.GetMouseButton(shootMouseButton) && !isshooting)
        {
            if (shootDirection.magnitude < 0.1f)
            {
                shootDirection = Vector2.right;
            }

            {
                StartCoroutine(bulletshooter(shootDirection)); // Start the coroutine and pass the direction 

            }
        }
    }




    IEnumerator bulletshooter(Vector2 bulletDirection)
    {


        isshooting = true;
        GameObject bullet = Instantiate(bulletprefab, transform.position, transform.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = bulletDirection * projectilespeed;
        Destroy(bullet, delay);
        yield return new WaitForSeconds(gunCoolDown);
        isshooting = false;
    }


    IEnumerator Dasher()
    {

        isdashing = true;
        myRigidbody.linearVelocity = myRigidbody.linearVelocity * dashStrength;
        yield return new WaitForSeconds(dashDuration);
        isdashing = false;

    }


    public void TakeDamage(float damage)
    {

        playerHealth = playerHealth - damage;
        if (playerHealth <= 0)
        {

            GameManager.Instance.GameOver();

        }


    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && Time.time >= nextDamageTime)
        {
            EnemyScript enemyScript = collision.gameObject.GetComponent<EnemyScript>();

            TakeDamage(enemyScript.enemyDamage);
            nextDamageTime = Time.time + damageTickRate;


            Debug.Log("player got hit");

        }

    }
}


    

