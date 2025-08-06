using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float attackInterval = 3f;
    public float timeBetweenShots = 0.5f;
    public float bossDamage = 10;

    private enum BossState { PhaseOne, PhaseTwo }
    private BossState currentState;

    void Start()
    {
        currentState = BossState.PhaseOne;
        StartCoroutine(BossAttackLoop());
    }

    IEnumerator BossAttackLoop()
    {
        while (true)
        {
            switch (currentState)
            {
                case BossState.PhaseOne:
                    yield return StartCoroutine(ScatterShotAttack());
                    currentState = BossState.PhaseTwo;
                    break;

                case BossState.PhaseTwo:
                    yield return StartCoroutine(ShotgunAttack());
                    currentState = BossState.PhaseOne;
                    break;
            }
            yield return new WaitForSeconds(attackInterval);
        }
    }

    IEnumerator ScatterShotAttack()
    {
       
        int bulletCount = 20;
        float angleStep = 360f / bulletCount;
        float currentAngle = 1f;

        for (int i = 0; i < bulletCount; i++)
        {
            float bulletDirX = transform.position.x + Mathf.Sin((currentAngle * Mathf.PI) / 180f);
            float bulletDirY = transform.position.y + Mathf.Cos((currentAngle * Mathf.PI) / 180f);

            Vector3 moveVector = new Vector3(bulletDirX, bulletDirY, 0f);
            Vector2 bulletDirection = (moveVector - transform.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = bulletDirection * 50f;

            currentAngle += angleStep;
            yield return new WaitForSeconds(timeBetweenShots / 2f);
        }
    }

    IEnumerator ShotgunAttack()
    {
     
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 playerDirection = (player.transform.position - transform.position).normalized;

            for (int i = 0; i < 5; i++)
            {
                float spreadAngle = Random.Range(-15f, 15f);
                Quaternion spreadRotation = Quaternion.Euler(0, 0, spreadAngle);
                Vector2 spreadDirection = spreadRotation * playerDirection;

                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().linearVelocity = spreadDirection * 40f;

                yield return new WaitForSeconds(timeBetweenShots);
            }
        }
    }
}