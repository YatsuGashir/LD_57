using UnityEngine;

public class EnemyRammer : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 20;  // Урон, который враг наносит платформе
    public GameObject explosionEffect;

    private Transform platform;
    private bool hasCollided;

    void Start()
    {
        platform = GameObject.FindGameObjectWithTag("Platform").transform;  // Находим платформу по тегу
    }

    void FixedUpdate()
    {
        if (!hasCollided && platform != null)
        {
            // Враг двигается к платформе
            Vector2 direction = (platform.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)  // Используем Collider2D для триггеров
    {
        Debug.Log("Trigger entered with: " + other.gameObject.name);
    
        if (other.CompareTag("Platform") && !hasCollided)
        {
            hasCollided = true;
            PlatformHealth platformHealth = other.GetComponent<PlatformHealth>();
        
            if (platformHealth != null)
            {
                platformHealth.TakeDamage(damage);
            }

            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}