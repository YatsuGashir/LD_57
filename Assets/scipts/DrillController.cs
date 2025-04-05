using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrillController : MonoBehaviour
{
    public static DrillController Instance;

    [Header("References")]
    [SerializeField] private GameObject platform;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private SpriteRenderer drillSpriteRenderer;
    [SerializeField] private BoxCollider2D platformCollider;

    [Header("Drill Settings")]
    [SerializeField] private float originalDrillSpeed = 0.005f;
    private float drillSpeed;

    [Header("Drill Pool")]
    public List<Drill> drillPool;
    public float currentDrillImprovement = 1f;

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
        drillSpeed = originalDrillSpeed;
        StartCoroutine(DrillDown());
    }

    public IEnumerator DrillDown()
    {
        while (true)
        {
            MovePlatformDown();
            yield return new WaitForSeconds(0.023f);
        }
    }

    private void MovePlatformDown()
    {
        Vector3 newPosition = platform.transform.position;
        newPosition.y -= drillSpeed * currentDrillImprovement;
        platform.transform.position = newPosition;

        CheckAndRemoveTilesUnderPlatform();
    }

    private void CheckAndRemoveTilesUnderPlatform()
    {
        if (platform == null || tilemap == null) return;

        platformCollider = platform.GetComponent<BoxCollider2D>();
        if (platformCollider == null) return;

        // Получаем границы коллайдера платформы в мировых координатах
        Bounds platformBounds = platformCollider.bounds;
    
        // Определяем область проверки (немного ниже платформы)
        float checkDepth = 0.2f;
        Vector3 checkStart = platformBounds.min - new Vector3(0, checkDepth, 0);
        Vector3 checkEnd = new Vector3(platformBounds.max.x, platformBounds.min.y - checkDepth, 0);

        // Переводим в координаты тайлмапа
        Vector3Int startCell = tilemap.WorldToCell(checkStart);
        Vector3Int endCell = tilemap.WorldToCell(checkEnd);

        // Проходим по всем ячейкам в области проверки
        for (int x = startCell.x; x <= endCell.x; x++)
        {
            for (int y = startCell.y; y <= endCell.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, startCell.z);
                if (tilemap.HasTile(cellPosition))
                {
                    tilemap.SetTile(cellPosition, null);
                    // Здесь можно добавить эффекты разрушения
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SoftRock"))
        {
            Debug.Log("Бур замедлен из-за земли");
            drillSpeed *= 0.5f;
            CameraShake.instance?.ShakeCamera(0.07f, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SoftRock"))
        {
            drillSpeed = originalDrillSpeed;
            CameraShake.instance?.ShakeCamera(0.01f, true);
        }
    }

    public void SelectDrill(int drillIndex)
    {
        if (drillIndex < 0 || drillIndex >= drillPool.Count) return;

        Drill selectedDrill = drillPool[drillIndex];
        currentDrillImprovement = selectedDrill.improvementFactor;
        
        if (drillSpriteRenderer != null)
        {
            drillSpriteRenderer.sprite = selectedDrill.drillSprite;
        }

        Debug.Log($"Выбрана дрель: {selectedDrill.drillName}, Коэффициент улучшения: {currentDrillImprovement}");
    }
}