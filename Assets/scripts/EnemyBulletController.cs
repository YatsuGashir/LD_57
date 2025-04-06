using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    public int damage = 5;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Platform"))
        {
            PlatformHealth health = collision.GetComponent<PlatformHealth>();
            if(health != null)
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if(collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}