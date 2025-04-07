using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrillController : MonoBehaviour
{
    public static DrillController Instance;

    [Header("References")]
    [SerializeField] private GameObject platform;
    [SerializeField] private Tilemap[] tilemaps; 
    [SerializeField] private SpriteRenderer drillSpriteRenderer;
    [SerializeField] private BoxCollider2D platformCollider;

    [Header("Drill Settings")]
    [SerializeField] private float originalDrillSpeed = 0.02f; // не используется в новой формуле
    public float drillSpeed;
    [SerializeField] private float fallSpeed = 0.1f; // стандартная скорость падения
    private bool isDrilling = false;
    private bool isCounting = false;
    private float currentTime;
    private int currentDrillIndex = 0;

    [Header("Drill Pool")]
    public List<Drill> drillPool;
    public float currentDrillImprovement = 1f; // оставляем для совместимости (можно использовать для вычислений бонусов, если потребуется)

    [Header("Overheat Settings")]
    [SerializeField] private float overheatDrillMultiplier = 0.1f; // коэффициент перегрева (не используется напрямую, но можно для расчёта)
    [SerializeField] private Color overheatColor = Color.red;
    [SerializeField] private ParticleSystem overheatParticles;
    [SerializeField] private AudioClip overheatSound;
    [SerializeField] private float baseDrillImprovement; // базовый коэффициент улучшения
    private bool isOverheated = false;
    private Color originalDrillColor;

    [Header("Shake Settings")]
    [SerializeField] private float shakeAmount = 0.1f; // амплитуда дрожания
    [SerializeField] private float shakeSpeed = 10f; // скорость дрожания

    private Vector3 originalDrillPosition;
    public bool isDrill = false;

    // Новые поля для расчёта скорости:
    [Header("Speed Modifiers")]
    [SerializeField] private float softRockResistance = 0.02f; // сопротивление мягкой породы
    [SerializeField] private float rockResistanceValue = 0.05f; // сопротивление обычного камня
    [SerializeField] private float hardRockResistance = 0.09f; // сопротивление для hard (будет давать почти нулевую скорость)
    [SerializeField] private float turretSpeedBonus = 0.05f; // бонус скорости от турели (применяется только если есть руда)
    [SerializeField] private float overheatPenaltyValue = 0.03f; // штраф скорости при перегреве

    private float currentRockResistance = 0f; // текущее сопротивление породы
    private bool hasOre = false; // флаг наличия руды (если руда добывается, бонус турели применяется)
    private bool isRocking = false; // для отслеживания состояния hard

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SelectDrill(0);
        // Изначально считаем скорость без сопротивления
        SumSpeed(0f);
        originalDrillColor = drillSpriteRenderer.color;
        baseDrillImprovement = currentDrillImprovement; // сохраняем базовое значение
        originalDrillPosition = drillSpriteRenderer.transform.localPosition; // сохраняем начальную позицию
        AudioManager.instance.Play("bigDrill", transform.position);
        AudioManager.instance.Play("ost");
    }

    private void FixedUpdate()
    {
        if (GameManager.instance != null && isDrill)
        {
            MovePlatformDown();
            CoolingSystem.instance?.DrainCooling(0.1f);
            ShakeDrill();
        }
    }

    public void MovePlatformDown()
    {
        Vector3 newPosition = platform.transform.position;
        newPosition.y -= drillSpeed;
        platform.transform.position = newPosition;

        CheckAndRemoveTilesUnderPlatform();
    }

    private void CheckAndRemoveTilesUnderPlatform()
    {
        if (platform == null) return;

        platformCollider = platform.GetComponent<BoxCollider2D>();
        if (platformCollider == null) return;

        Bounds platformBounds = platformCollider.bounds;
        float checkDepth = 0.2f;
        Vector3 checkStart = platformBounds.min - new Vector3(0, checkDepth, 0);
        Vector3 checkEnd = new Vector3(platformBounds.max.x, platformBounds.min.y - checkDepth, 0);

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap != null)
            {
                CheckTilemap(tilemap, checkStart, checkEnd);
            }
        }
    }
    
    private void CheckTilemap(Tilemap tilemap, Vector3 checkStart, Vector3 checkEnd)
    {
        if (tilemap == null) return;

        Vector3Int startCell = tilemap.WorldToCell(checkStart);
        Vector3Int endCell = tilemap.WorldToCell(checkEnd);

        for (int x = startCell.x; x <= endCell.x; x++)
        {
            for (int y = startCell.y; y <= endCell.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, startCell.z);
                if (tilemap.HasTile(cellPosition))
                {
                    tilemap.SetTile(cellPosition, null);
                }
            }
        }
    }

    // При входе в зону разного типа породы устанавливаем соответствующее сопротивление:
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SoftRock"))
        {
            Debug.Log("Бур замедлен из-за мягкой породы");
            currentRockResistance = softRockResistance;
            SumSpeed(currentRockResistance);
            CameraShake.instance?.ShakeCamera(0.07f, true);
        }
        if (other.CompareTag("Rock"))
        {
            Debug.Log("Бур замедлен из-за камня");
            currentRockResistance = rockResistanceValue;
            SumSpeed(currentRockResistance);
            CameraShake.instance?.ShakeCamera(0.07f, true);
        }
        if (other.CompareTag("Hard"))
        {
            TriggerEnemiesEscape();
            currentTime = 7f;
            Debug.Log("Бур погряз в очень жёсткой породе");
            // Применяем максимальное сопротивление для hard
            currentRockResistance = hardRockResistance;
            SumSpeed(currentRockResistance);
            CameraShake.instance?.ShakeCamera(0.01f, true);
            GameManager.instance.MiningStage();
            DroneController.instance.isActive = true;
            if (isRocking)
            {
                TutorialManager.instance.HardGround();
                isRocking = false;
            }
            StartCoroutine(HardDrill());
        }
    }

    // Во время нахождения в области породы можем обновлять камеру
    private void OnTriggerStay2D(Collider2D other)
    {
        if ((other.CompareTag("SoftRock") || other.CompareTag("Rock")) && GameManager.instance.isDrillingActive)
        {
            CameraShake.instance?.ShakeCamera(0.07f, true);
        }
        if (other.CompareTag("Hard") && GameManager.instance.isDrillingActive)
        {
            CameraShake.instance?.ShakeCamera(0.01f, true);
        }
    }

    // После выхода из зоны сопротивления сбрасываем сопротивление на 0
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SoftRock") || other.CompareTag("Rock") || other.CompareTag("Hard"))
        {
            // Для hard оставляем флаг, если нужно повторное применение эффекта
            if (other.CompareTag("Hard"))
                isRocking = true;
            currentRockResistance = 0f;
            SumSpeed(currentRockResistance);
            CameraShake.instance?.ShakeCamera(0.01f, true);
        }
    }

    // Новый метод для пересчёта скорости по заданной формуле
    private void SumSpeed(float rockResistance)
    {
        // Если руда добывается, бонус от турели применяется, иначе нет.
        float turretBonus = hasOre ? turretSpeedBonus : 0f;
        // Штраф за перегрев, если бур перегрет
        float overheatPenalty = isOverheated ? overheatPenaltyValue : 0f;

        drillSpeed = fallSpeed - rockResistance + turretBonus - overheatPenalty;
        // Не допускаем отрицательной скорости
        if (drillSpeed < 0f) drillSpeed = 0f;
    }

    // Метод для выбора бура
    public void SelectDrill(int drillIndex)
    {
        if (drillIndex < 0 || drillIndex >= drillPool.Count) return;

        currentDrillIndex = drillIndex;
        Drill selectedDrill = drillPool[drillIndex];
        baseDrillImprovement = selectedDrill.improvementFactor; // обновляем базовое значение
        // Если бур перегрет, коэффициент снижается (это можно использовать для других вычислений)
        currentDrillImprovement = isOverheated ? baseDrillImprovement * overheatDrillMultiplier : baseDrillImprovement;

        // Пересчитываем скорость с текущим сопротивлением
        SumSpeed(currentRockResistance);

        if (drillSpriteRenderer != null)
        {
            drillSpriteRenderer.sprite = selectedDrill.drillSprite;
        }

        Debug.Log($"Выбрана дрель: {selectedDrill.drillName}, Коэффициент улучшения: {currentDrillImprovement}");
    }

    // Метод для установки состояния наличия руды; бонус турели применяется только если руда добывается.
    public void SetOreStatus(bool orePresent)
    {
        hasOre = orePresent;
        SumSpeed(currentRockResistance);
    }

    public void CheckOverheat(float currentCooling)
    {
        if (currentCooling <= 0 && !isOverheated)
        {
            StartOverheat();
        }
        else if (currentCooling > 0 && isOverheated)
        {
            StopOverheat();
        }
    }

    private void StartOverheat()
    {
        isOverheated = true;
        // При перегреве можно дополнительно изменять коэффициенты, если нужно (например, уменьшать улучшение)
        if (drillSpriteRenderer != null)
        {
            drillSpriteRenderer.color = overheatColor;
        }
        
        if (overheatParticles != null)
        {
            overheatParticles.Play();
        }
        
        if (overheatSound != null)
        {
            AudioSource.PlayClipAtPoint(overheatSound, transform.position);
        }
        
        Debug.Log("Перегрев! Мощность снижена");
        // Пересчитываем скорость с учетом перегрева
        SumSpeed(currentRockResistance);
    }

    private void StopOverheat()
    {
        isOverheated = false;
        currentDrillImprovement = baseDrillImprovement;
        
        if (drillSpriteRenderer != null)
        {
            drillSpriteRenderer.color = originalDrillColor;
        }
        
        if (overheatParticles != null)
        {
            overheatParticles.Stop();
        }
        
        Debug.Log("Охлаждение восстановлено. Мощность нормальная");
        SumSpeed(currentRockResistance);
    }

    // Метод для дрожания бура
    private void ShakeDrill()
    {
        float shakeOffset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        drillSpriteRenderer.transform.localPosition = originalDrillPosition + new Vector3(0f, shakeOffset - 0.2f, 0f);
    }
    
    private void TriggerEnemiesEscape()
    {
        EnemyShooter[] enemiesSh = FindObjectsOfType<EnemyShooter>();
        foreach (EnemyShooter enemy in enemiesSh)
        {
            enemy.EscapeAndDespawn();
        }
        EnemyRammer[] enemiesRm = FindObjectsOfType<EnemyRammer>();
        foreach (EnemyRammer enemy in enemiesRm)
        {
            enemy.EscapeAndDespawn();
        }
        EnemySpawnEnemy[] enemiesSp = FindObjectsOfType<EnemySpawnEnemy>();
        foreach (EnemySpawnEnemy enemy in enemiesSp)
        {
            enemy.EscapeAndDespawn();
        }
    }

    // Специальная логика для hard-породы (снижает скорость почти до нуля в течение 7 секунд)
    private IEnumerator HardDrill()
    {
        while (currentTime > 0)
        {
            DroneHUD.instance.TimeUpdate(currentTime);
            currentTime -= Time.deltaTime;
            yield return null;
        }

        DroneHUD.instance.TimeUpdate(-1f);
        CameraShake.instance?.ShakeCamera(0f, false);
        // По окончании hard-процесса сбрасываем сопротивление
        currentRockResistance = 0f;
        SumSpeed(currentRockResistance);
    }
}
