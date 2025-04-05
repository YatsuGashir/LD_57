using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Настройки поворота")]
    public float maxAngle = 45f; // максимум вверх/влево/вправо
    public Transform turretPivot; // часть, которая поворачивается
    public float maxRotationAngle = 45f; // Максимальный угол поворота в градусах

    [Header("Настройки стрельбы")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    void Update()
    {
        RotateTowardsMouse();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
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
        rb.velocity = firePoint.right * bulletSpeed;
    }
}