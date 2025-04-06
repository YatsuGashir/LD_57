using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 10; // Урон пули
    public string enemyTag = "Enemy"; // Тег врага

    void Start()
    {
        // Уничтожить пулю через несколько секунд (если не столкнулась с чем-то)
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, если пуля столкнулась с врагом
        if (collision.CompareTag(enemyTag))
        {
            // Наносим урон врагу
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage); // Наносим урон
            }

            Destroy(gameObject); // Уничтожаем пулю
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject); // Уничтожаем пулю при столкновении со стеной
        }
    }
}