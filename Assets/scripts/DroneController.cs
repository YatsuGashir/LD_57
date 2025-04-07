using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DroneController : MonoBehaviour
{
    public static DroneController instance;
    [SerializeField] private ParticleSystem miningParticle;
    
    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject box;
    [SerializeField] private Tilemap oreTilemap;
    [SerializeField] private float checkRadius = 0.5f;

    [Header("Настройки анимации разрушения")]
    [SerializeField] private float shakeDuration = 0.7f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private GameObject destroyEffect;

    [Header("Настройки ввода")]
    [SerializeField] private float modeSwitchCooldown = 0.5f;
    
    [Header("Настройки расстояния дро платформы")]
    [SerializeField] private GameObject platform;
    [SerializeField] private float switchRadius = 2f;

    private Vector3 targetPosition;
    public bool isActive = false;
    private float lastSwitchTime = -1f;
    
    private bool firsOre = false;
    private bool firstSit = false;
    public int oreCount = 0; // Счётчик руды

    [Header("Дрожание бура")]
    [SerializeField] private Transform drillLeftTransform; // Ссылка на дочерний объект бура
    [SerializeField] private Transform drillRightTransform;
    [SerializeField] private float drillShakeIntensity = 0.1f;
    private Coroutine shakeCoroutine;
    private Vector3 originalDrillLeftPosition;
    private Vector3 originalDrillRightPosition;

    [Header("Настройки наклона дрона")]
    [SerializeField] private float tiltMultiplier = 5f;   // Насколько сильно будет влиять разница по X на угол наклона
    [SerializeField] private float maxTiltAngle = 15f;      // Максимальный угол наклона (в градусах)
    [SerializeField] private float rotationSpeed = 5f;      // Скорость плавного наклона

    private void Awake()
    {
        instance = this;
        originalDrillLeftPosition = drillLeftTransform.localPosition;
        originalDrillRightPosition = drillRightTransform.localPosition;
    }
    
    private void Start()
    {
        miningParticle.Stop();
        // Синхронизируем состояние с GameManager
        isActive = GameManager.instance.isDrillingActive;
    
        // Если нужно переопределить:
        isActive = true; // Дрон начинает активным
        GameManager.instance.MiningStage(); // Принудительно включаем режим майнинга
    }
    
    private void Update()
    {
        HandleModeSwitch();
        HandleMovement();
        CheckAndDestroyOre();
    }
    
    private void FixedUpdate()
    {
        MoveTowardsTarget();
    }
    
    private void HandleModeSwitch()
    {
        if (Input.GetMouseButton(1) && Time.time - lastSwitchTime >= modeSwitchCooldown)
        {
            lastSwitchTime = Time.time;

            if (isActive) // Сейчас активен дрон → хотим вернуться в режим бурения
            {
                float distanceToPlatform = Vector3.Distance(transform.position, platform.transform.position);
                if (distanceToPlatform > switchRadius)
                {
                    Debug.Log("Слишком далеко от платформы для возвращения в режим бурения.");
                    return;
                }

                isActive = false;
                if (!firstSit)
                {
                    TutorialManager.instance.firstSit();
                    firstSit = true;
                }
                GameManager.instance.DrillingStage();
            }
            else // Сейчас активна платформа → переходим в майнинг
            {
                isActive = true;
                GameManager.instance.MiningStage();
            }
        }
    }
    
    // Изменённый метод HandleMovement: теперь дрон вычисляет целевую позицию и наклоняет свою верхнюю часть в сторону курсора
    private void HandleMovement()
    {
        Vector3 cursorPosition;
        if (isActive)
        {
            cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            cursorPosition = box.transform.position + Vector3.down * 1f;
        }
        cursorPosition.z = 0f;
        targetPosition = cursorPosition;

        // Вычисляем горизонтальную разницу между позицией курсора и дрона
        float horizontalDiff = targetPosition.x - transform.position.x;
        // Инвертируем значение, чтобы верх дрона наклонялся в сторону курсора
        float targetTilt = Mathf.Clamp(-horizontalDiff * tiltMultiplier, -maxTiltAngle, maxTiltAngle);
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetTilt);
        // Плавно приближаем текущий угол к целевому
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
    
    private void CheckAndDestroyOre()
    {
        Vector3Int currentCell = oreTilemap.WorldToCell(transform.position);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int checkPosition = new Vector3Int(
                    currentCell.x + x,
                    currentCell.y + y,
                    currentCell.z
                );

                TileBase tile = oreTilemap.GetTile(checkPosition);
                if (tile != null)
                {
                    miningParticle.Play();
                    StartCoroutine(PlayDestroyAnimation(
                        oreTilemap.CellToWorld(checkPosition),
                        (tile as Tile).sprite
                    ));

                    // Запускаем дрожание бура
                    if (shakeCoroutine != null)
                        StopCoroutine(shakeCoroutine);
                    shakeCoroutine = StartCoroutine(ShakeDrill());

                    oreTilemap.SetTile(checkPosition, null);
                    oreCount++;
                    UpgradeManager.instance.AddOre(1);
                    return;
                }
            }
        }
    }
    
    private IEnumerator ShakeDrill()
    {
        if (drillLeftTransform == null && drillRightTransform == null) yield break;

        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            float xShake = Random.Range(-drillShakeIntensity, drillShakeIntensity);
            drillLeftTransform.localPosition = originalDrillLeftPosition + new Vector3(xShake, 0, 0);
            drillRightTransform.localPosition = originalDrillRightPosition + new Vector3(xShake, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        drillLeftTransform.localPosition = originalDrillLeftPosition;
        drillRightTransform.localPosition = originalDrillRightPosition;
    }
    
    private IEnumerator PlayDestroyAnimation(Vector3 position, Sprite oreSprite)
    {
        Vector3 cellCenter = oreTilemap.GetCellCenterWorld(oreTilemap.WorldToCell(position));
    
        GameObject tempOre = new GameObject("TempOre");
        tempOre.transform.position = cellCenter;
        SpriteRenderer renderer = tempOre.AddComponent<SpriteRenderer>();
        renderer.sprite = oreSprite;
        renderer.sortingOrder = 1;

        float shakeTimer = 0f;
        Vector3 startPosition = tempOre.transform.position;

        while (shakeTimer < shakeDuration)
        {
            shakeTimer += Time.deltaTime;
            float yShake = Random.Range(-shakeIntensity, shakeIntensity);
            tempOre.transform.position = startPosition + new Vector3(0, yShake, 0);
            yield return null;
        }

        tempOre.transform.position = startPosition;

        float fadeTimer = 0f;
        Color startColor = renderer.color;

        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            renderer.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration)
            );
            yield return null;
        }

        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, startPosition, Quaternion.identity);
        }
        if (!firsOre)
        {
            TutorialManager.instance.firstOreEx();
            firsOre = true;
        }
        miningParticle.Stop();
        Destroy(tempOre);
    }
    
    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("CoolingStation"))
        {
            CoolingSystem.instance.RefillCooling(2f);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (platform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(platform.transform.position, switchRadius);
        }
    }
}
