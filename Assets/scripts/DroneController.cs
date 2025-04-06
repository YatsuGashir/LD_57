using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DroneController : MonoBehaviour
{
    public static DroneController instance;

    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject box;
    [SerializeField] private Tilemap oreTilemap;
    [SerializeField] private float checkRadius = 0.5f;

    [Header("Настройки анимации разрушения")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private GameObject destroyEffect;

    [Header("Настройки ввода")]
    [SerializeField] private float modeSwitchCooldown = 0.5f;

    private Vector3 targetPosition;
    public bool isActive = false;
    private float lastSwitchTime = -1f;

    public int oreCount = 0; // Счётчик руды

    private void Awake()
    {
        instance = this;
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
            isActive = !isActive;

            // Заменяем SwitchStage(isActive) на прямые вызовы
            if (isActive)
            {
                GameManager.instance.MiningStage();
            }
            else
            {
                GameManager.instance.DrillingStage();
            }
        }
    }

    private void HandleMovement()
    {
        Vector3 cursorPosition = isActive ? 
            Camera.main.ScreenToWorldPoint(Input.mousePosition) : 
            box.transform.position;

        cursorPosition.z = 0f;
        targetPosition = cursorPosition;
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
                    StartCoroutine(PlayDestroyAnimation(
                        oreTilemap.CellToWorld(checkPosition),
                        (tile as Tile).sprite
                    ));

                    oreTilemap.SetTile(checkPosition, null);

                    // Увеличиваем счётчик руды и передаем её в UpgradeManager
                    oreCount++;
                    UpgradeManager.instance.AddOre(1);  // 1 руда добавляется за разрушение

                    return;
                }
            }
        }
    }

    private IEnumerator PlayDestroyAnimation(Vector3 position, Sprite oreSprite)
    {
        // Создаем временный объект для анимации
        GameObject tempOre = new GameObject("TempOre");
        tempOre.transform.position = position;
        SpriteRenderer renderer = tempOre.AddComponent<SpriteRenderer>();
        renderer.sprite = oreSprite;
        renderer.sortingOrder = 1;

        // Эффект дрожания
        float shakeTimer = 0f;
        Vector3 startPosition = tempOre.transform.position;

        while (shakeTimer < shakeDuration)
        {
            shakeTimer += Time.deltaTime;
            tempOre.transform.position = startPosition + Random.insideUnitSphere * shakeIntensity;
            yield return null;
        }
        tempOre.transform.position = startPosition;

        // Эффект исчезновения
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

        // Визуальные эффекты
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, position, Quaternion.identity);
        }

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
            CoolingSystem.instance.RefillCooling(2f); // 2 единицы за кадр
        }
    }
    
}
