using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Стрельба")]
    public GameObject enemyBulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletSpeed = 5f;
    
    [Header("Передвижение")]
    public float movementSpeed = 3f;
    public float minDistance = 2f;
    public float maxDistance = 5f;
    public float stoppingDistance = 3f;
    public float sideMovementSpeed = 2f; // Скорость бокового движения
    public float directionChangeInterval = 2f; // Интервал смены направления
    
    private Transform platform;
    private Rigidbody2D rb;
    private float nextFireTime;
    private float directionTimer;
    private int currentDirection = 1; // 1 - вправо, -1 - влево
    private bool isInOptimalZone;

    void Start()
    {
        platform = GameObject.FindGameObjectWithTag("Platform").transform;
        rb = GetComponent<Rigidbody2D>();
        nextFireTime = Time.time + fireRate;
        directionTimer = directionChangeInterval;
    }

    void Update()
    {
        if(platform == null) return;

        HandleMovement();
        HandleShooting();
        HandleSideMovement();
    }

    void HandleMovement()
    {
        float distanceToPlatform = Vector2.Distance(transform.position, platform.position);
        isInOptimalZone = distanceToPlatform > stoppingDistance - 0.5f && 
                        distanceToPlatform < stoppingDistance + 0.5f;

        if(distanceToPlatform > maxDistance)
        {
            Vector2 direction = (platform.position - transform.position).normalized;
            rb.linearVelocity = direction * movementSpeed;
        }
        else if(distanceToPlatform < minDistance)
        {
            Vector2 direction = (transform.position - platform.position).normalized;
            rb.linearVelocity = direction * movementSpeed;
        }
        else if(isInOptimalZone)
        {
            // Сохраняем вертикальную позицию
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }

    void HandleSideMovement()
    {
        if(!isInOptimalZone) return;

        directionTimer -= Time.deltaTime;
        
        if(directionTimer <= 0)
        {
            currentDirection *= -1;
            directionTimer = Random.Range(directionChangeInterval * 0.5f, 
                                        directionChangeInterval * 1.5f);
        }

        // Боковое движение с учетом препятствий
        Vector2 sideMovement = transform.right * currentDirection * sideMovementSpeed;
        rb.linearVelocity = new Vector2(sideMovement.x, rb.linearVelocity.y);

        // Поворот спрайта
        transform.localScale = new Vector3(currentDirection, 1, 1);
    }

    void HandleShooting()
    {
        if(!isInOptimalZone) return;

        if(Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        Vector2 direction = (platform.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = direction * bulletSpeed;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}