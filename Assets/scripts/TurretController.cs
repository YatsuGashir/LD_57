using UnityEngine;

public class TurretController : MonoBehaviour
{
    public static TurretController instance;

    [Header("Настройки поворота")]
    public float maxAngle = 45f;
    public Transform turretPivot;
    public float maxRotationAngle = 45f;
    public float rotationSpeed = 5f;

    [Header("Настройки стрельбы")] 
    [SerializeField] private ParticleSystem shootParticle;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float baseBulletSpeed = 10f;
    public float baseFireRate = 0.1f;

    private float fireRate;
    private float bulletSpeed;
    private float nextFireTime = 0f;

    [Header("Улучшения турели")]
    public TurretUpgrade[] upgrades;
    [SerializeField] private SpriteRenderer turretRenderer;
    private int currentUpgradeIndex = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        shootParticle.Stop();
        ApplyUpgrade(currentUpgradeIndex);
    }

    void FixedUpdate()
    {
        if (GameManager.instance == null || GameManager.instance.isDrillingActive)
        {
            RotateTowardsMouse();

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

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        targetAngle = Mathf.Clamp(targetAngle, -maxRotationAngle, maxRotationAngle);

        float currentAngle = turretPivot.eulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360;

        float newAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        turretPivot.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    void Shoot()
    {
        shootParticle.Play();
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = firePoint.up * bulletSpeed;
        }
    }

    public void UpgradeTurret(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length) return;

        currentUpgradeIndex = upgradeIndex;
        ApplyUpgrade(upgradeIndex);
    }

    private void ApplyUpgrade(int upgradeIndex)
    {
        TurretUpgrade upgrade = upgrades[upgradeIndex];
        fireRate = baseFireRate / upgrade.fireRateMultiplier;
        bulletSpeed = baseBulletSpeed * upgrade.bulletSpeedMultiplier;

        if (turretRenderer != null && upgrade.upgradeSprite != null)
        {
            turretRenderer.sprite = upgrade.upgradeSprite;
        }

        Debug.Log($"Турель улучшена: {upgrade.upgradeName}");
    }
}
