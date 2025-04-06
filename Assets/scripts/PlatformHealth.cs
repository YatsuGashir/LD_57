using UnityEngine;

public class PlatformHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    //public UnityEvent OnDamageTaken;
    //public UnityEvent OnDestroyed;
    [SerializeField] private PlatformBar platformBar;  // Ссылка на UI панель здоровья

    void Start()
    {
        currentHP = maxHP;
        platformBar.SetMaxBar(maxHP);  // Инициализация максимального значения на UI
    }

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        platformBar.SetBar(currentHP);  // Обновляем UI

        //OnDamageTaken.Invoke();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //OnDestroyed.Invoke();
        // Добавьте логику уничтожения/отключения платформы
        Destroy(gameObject);
    }
}