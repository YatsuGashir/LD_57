using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 50;
    public int currentHP;
    public string enemyTag = "Enemy"; // Убедитесь, что враги имеют тег "Enemy"

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        // Проверка на смерть врага
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Добавить эффекты смерти (например, взрыв) или другие действия
        Destroy(gameObject); // Уничтожаем врага
    }

    // Этот метод срабатывает при столкновении с пулей
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем, что столкновение с пулей
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletController bullet = collision.gameObject.GetComponent<BulletController>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage); // Наносим урон врагу от пули
            }
        }
    }
}