using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Настройки поворота")]
    public float maxAngle = 45f; // максимум вверх/влево/вправо
    public Transform turretPivot; // часть, которая поворачивается
    public float maxRotationAngle = 45f; // Максимальный угол поворота в градусах

    [Header("Настройки стрельбы")]
    public GameObject bulletPrefab;
    public Transform firePoint; // Позиция, откуда вылетает пуля
    public float bulletSpeed = 10f;
    public float fireRate = 0.1f; // Интервал между выстрелами (в секундах)
    
    private float nextFireTime = 0f;

    void Update()
    {
        RotateTowardsMouse();

        // Проверяем зажатие ЛКМ и таймер между выстрелами
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // Устанавливаем время следующего выстрела
        }
    }

    void RotateTowardsMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mouseWorldPos - turretPivot.position;
        direction.z = 0f;

        // Угол между осью X и направлением на курсор
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Корректируем, потому что твоя турель смотрит вверх (а не вправо)
        angle -= 90f;

        // Ограничиваем угол вращения в пределах ±45° от вертикали
        angle = Mathf.Clamp(angle, -maxRotationAngle, maxRotationAngle);

        // Применяем вращение
        turretPivot.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = firePoint.up * bulletSpeed;
        }
    }
}