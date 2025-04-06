using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Стрельба")]
    public GameObject enemyBulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletSpeed = 5f;

    [Header("Движение")]
    public float movementSpeed = 3f;         // скорость сближения
    public float sideMovementSpeed = 2f;     // скорость бокового движения
    public float maxDistance = 6f;           // если дальше — сближаемся
    public float directionChangeInterval = 2f;

    private Transform platform;
    private Rigidbody2D rb;
    private float nextFireTime;
    private float directionTimer;
    private int currentDirection = 1;

    void Start()
    {
        platform = GameObject.FindGameObjectWithTag("Platform")?.transform;
        rb = GetComponent<Rigidbody2D>();
        nextFireTime = Time.time + fireRate;
        directionTimer = directionChangeInterval;
    }

    void FixedUpdate()
    {
        if (platform == null) return;

        Vector2 toPlatform = platform.position - transform.position;
        float distance = toPlatform.magnitude;
        Vector2 directionToPlatform = toPlatform.normalized;

        Vector2 finalVelocity = Vector2.zero;

        // Если далеко от платформы — подлетаем ближе
        if (distance > maxDistance)
        {
            finalVelocity += directionToPlatform * movementSpeed;
        }

        // Движение по касательной (влево-вправо)
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
        {
            currentDirection *= -1;
            directionTimer = Random.Range(directionChangeInterval * 0.5f, directionChangeInterval * 1.5f);
        }

        Vector2 sideDirection = Vector2.Perpendicular(directionToPlatform) * currentDirection;
        finalVelocity += sideDirection * sideMovementSpeed;

        rb.linearVelocity = finalVelocity;

        // Поворот спрайта по направлению движения
        transform.localScale = new Vector3(currentDirection, 1, 1);

        HandleShooting();
    }

    void HandleShooting()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (enemyBulletPrefab == null || firePoint == null) return;

        Vector2 shootDir = (platform.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = shootDir * bulletSpeed;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
