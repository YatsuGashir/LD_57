using UnityEngine;

public class EnemySpawnEnemy : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public float spawnCheckRadius = 1f;
    public Vector3 spawnOffset = new Vector3(0, -1f, 0);

    [Header("Movement Settings")]
    public float movementSpeed = 3f;
    public float sideMovementSpeed = 2f;
    public float maxDistance = 6f;
    public float directionChangeInterval = 2f;

    [Header("Gizmos")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.red;

    private Transform platform;
    private Rigidbody2D rb;
    private float nextSpawnTime;
    private float directionTimer;
    private int currentDirection = 1;
    private LayerMask obstacleMask;

    void Start()
    {
        platform = GameObject.FindGameObjectWithTag("Platform")?.transform;
        rb = GetComponent<Rigidbody2D>();
        obstacleMask = LayerMask.GetMask("Obstacle", "Ground");
        directionTimer = directionChangeInterval;
        nextSpawnTime = Time.time + spawnInterval;
    }

    void FixedUpdate()
    {
        if (platform == null) return;

        HandleMovement();
        HandleSpawnerLogic();
    }

    void HandleMovement()
    {
        Vector2 toPlatform = platform.position - transform.position;
        float distance = toPlatform.magnitude;
        Vector2 directionToPlatform = toPlatform.normalized;

        Vector2 finalVelocity = Vector2.zero;

        // Основное движение к платформе
        if (distance > maxDistance)
        {
            finalVelocity += directionToPlatform * movementSpeed;
        }

        // Боковое движение
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
        {
            currentDirection *= -1;
            directionTimer = Random.Range(
                directionChangeInterval * 0.5f, 
                directionChangeInterval * 1.5f
            );
        }

        Vector2 sideDirection = Vector2.Perpendicular(directionToPlatform) * currentDirection;
        finalVelocity += sideDirection * sideMovementSpeed;

        rb.linearVelocity = finalVelocity;
        transform.localScale = new Vector3(currentDirection, 1, 1);
    }

    void HandleSpawnerLogic()
    {
        if (Time.time >= nextSpawnTime)
        {
            TrySpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void TrySpawnEnemy()
    {
        if (enemyPrefab == null) return;

        Vector3 spawnPosition = transform.position + 
            new Vector3(
                spawnOffset.x * currentDirection, 
                spawnOffset.y, 
                spawnOffset.z
            );

        if (IsSpawnPositionValid(spawnPosition))
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    bool IsSpawnPositionValid(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            position, 
            spawnCheckRadius, 
            obstacleMask
        );

        return colliders.Length == 0;
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = gizmoColor;
        Vector3 spawnPos = transform.position + spawnOffset;
        Gizmos.DrawWireSphere(spawnPos, spawnCheckRadius);
        Gizmos.DrawLine(transform.position, spawnPos);
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
