using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Настройки поворота")]
    public float maxAngle = 45f; // максимум вверх/влево/вправо
    public Transform turretPivot; // часть, которая поворачивается
    public float maxRotationAngle = 45f; // Максимальный угол поворота в градусах
    public float rotationSpeed = 5f; // Скорость поворота (градусов в секунду)

    [Header("Настройки стрельбы")]
    public GameObject bulletPrefab;
    public Transform firePoint; // Позиция, откуда вылетает пуля
    public float bulletSpeed = 10f;
    public float fireRate = 0.1f; // Интервал между выстрелами (в секундах)
    
    private float nextFireTime = 0f;

    void Update()
    {
        // Проверяем активную стадию перед обработкой ввода
        if (GameManager.instance == null || GameManager.instance.isDrillingActive)
        {
            RotateTowardsMouse();
            
            // Проверяем зажатие ЛКМ и таймер между выстрелами
            if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void RotateTowardsMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mouseWorldPos - turretPivot.position;
        direction.z = 0f;

        // Угол между осью X и направлением на курсор
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Ограничиваем угол вращения
        targetAngle = Mathf.Clamp(targetAngle, -maxRotationAngle, maxRotationAngle);

        // Плавный поворот с заданной скоростью
        float currentAngle = turretPivot.eulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360; // Корректируем угол для плавного вращения
        
        float newAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        
        // Применяем вращение
        turretPivot.rotation = Quaternion.Euler(0, 0, newAngle);
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