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
    [SerializeField] private float originalDrillSpeed = 0.005f;
    private float drillSpeed;
    private bool isDrilling = false;
    private bool  isCounting = false;
    private float currentTime;

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
        //StartCoroutine(DrillDown());
    }

    public IEnumerator DrillDown()
    {
        while (true)
        {
            MovePlatformDown();
            CoolingSystem.instance.UpdateCoolingVolume(0.01f);
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
                    // Эффекты разрушения можно добавить здесь
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

        if (other.CompareTag("Hard"))
        {
            currentTime = 7f;
            Debug.Log("Бур погряз в очень жёсткой породе");
            drillSpeed *= 0.01f;
            CameraShake.instance?.ShakeCamera(1f, true);
            GameManager.instance.MiningStage();
            DroneController.instance.isActive = true;
            StartCoroutine(HardDrill());
        }
    }

    private IEnumerator HardDrill()
    {
       
    
        while (currentTime > 0)
        {
            DroneHUD.instance.TimeUpdate(currentTime);
            currentTime -= Time.deltaTime;
            yield return null;
        }

        // После завершения таймера:
        DroneHUD.instance.TimeUpdate(-1f);
        drillSpeed = originalDrillSpeed; // Восстанавливаем полную скорость
        CameraShake.instance?.ShakeCamera(0f, false); // Останавливаем тряску камеры
        GameManager.instance.DrillingStage(); // Возвращаемся в режим бурения
        DroneController.instance.isActive = false; // Деактивируем дрон
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SoftRock"))
        {
            drillSpeed = originalDrillSpeed;
            CameraShake.instance?.ShakeCamera(0.01f, true);
        }
        if (other.CompareTag("Hard"))
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