using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 1;
    public string enemyTag = "Enemy";

    void Start()
    {
        // Уничтожить пулю через несколько секунд (если никуда не врезалась)
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Если это враг
        if (collision.CompareTag(enemyTag))
        {
            // Пробуем получить компонент здоровья
            //EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            /*if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }*/

            Destroy(gameObject); // Уничтожаем пулю
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject); // Врезалась в стену — уничтожить
        }
    }
}
