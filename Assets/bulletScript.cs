using UnityEngine;

public class BulletScript : MonoBehaviour

{

    public int damageAmount = 1; // How much damage this bullet does
    public GameObject Enemy;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyScript EnemyScript = other.gameObject.GetComponent<EnemyScript>();
            EnemyScript.TakeDamage(damageAmount);
            Destroy(gameObject);
            Debug.Log("bullet hit player");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
}
