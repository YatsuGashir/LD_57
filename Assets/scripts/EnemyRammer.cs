using System.Collections;
using UnityEngine;

public class EnemyRammer : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 20;
    public GameObject explosionEffect;

    [Header("Spiral Settings")]
    public float spiralStartRadius = 3f;       // Начальный радиус спирали
    public float spiralRotationSpeed = 5f;     // Скорость вращения по кругу
    public float spiralTighteningSpeed = 1f;   // Насколько быстро спираль "сужается"
    public float spiralForwardOffset = 2f;  // Насколько вперёд по направлению крутится враг
    
    [Header("Побег")]
    [SerializeField] private float escapeSpeed = 10f;
    [SerializeField] private float escapeDuration = 2f;
    private bool isEscaping = false;

    private Transform platform;
    private bool hasCollided;
    private float angle;
    private float currentRadius;

    void Start()
    {
        platform = GameObject.FindGameObjectWithTag("Platform").transform;
        angle = 0f;
        currentRadius = spiralStartRadius;
    }

    void FixedUpdate()
    {
        if (isEscaping) return;
        if (!hasCollided && platform != null)
        {
            MoveInSpiral();
        }
    }

    public void EscapeAndDespawn()
    {
        if (!isEscaping)
        {
            StartCoroutine(EscapeRoutine());
        }
    }

    private IEnumerator EscapeRoutine()
    {
        isEscaping = true;
        GetComponent<Collider2D>().enabled = false; // Отключаем коллайдер
        Destroy(GetComponent<Rigidbody2D>()); // Удаляем физику

        float timer = 0f;
        Vector3 startPos = transform.position;

        while (timer < escapeDuration)
        {
            timer += Time.deltaTime;
            transform.position += Vector3.up * escapeSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
    
    void MoveInSpiral()
    {
        // Центр окружности: точка вдоль пути к платформе
        Vector2 toPlatform = (platform.position - transform.position).normalized;
        Vector2 spiralCenter = (Vector2)transform.position + toPlatform * spiralForwardOffset;

        // Увеличиваем угол по времени
        angle += spiralRotationSpeed * Time.deltaTime;

        // Радиус уменьшается (если нужно)
        spiralStartRadius = Mathf.Max(0.1f, spiralStartRadius - spiralTighteningSpeed * Time.deltaTime);

        // Положение на окружности вокруг центра
        float x = Mathf.Cos(angle) * spiralStartRadius;
        float y = Mathf.Sin(angle) * spiralStartRadius;
        Vector2 offset = new Vector2(x, y);

        // Новая позиция — центр + смещение
        Vector2 targetPosition = spiralCenter + offset;

        // Двигаемся к новой позиции
        Vector2 moveDir = (targetPosition - (Vector2)transform.position).normalized;
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);

        // Поворачиваем врага по движению
       /* float rotateAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.AngleAxis(rotateAngle, Vector3.forward),
            5f * Time.deltaTime
        );*/
       
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spiralStartRadius);
    }
}
