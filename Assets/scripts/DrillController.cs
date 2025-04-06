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
    [SerializeField] private float originalDrillSpeed = 0.02f;
    private float drillSpeed;
    private bool isDrilling = false;
    private bool isCounting = false;
    private float currentTime;
    private int currentDrillIndex = 0;

    [Header("Drill Pool")]
    public List<Drill> drillPool;
    public float currentDrillImprovement = 1f;

    [Header("Overheat Settings")]
    [SerializeField] private float overheatDrillMultiplier = 0.1f;
    [SerializeField] private Color overheatColor = Color.red;
    [SerializeField] private ParticleSystem overheatParticles;
    [SerializeField] private AudioClip overheatSound;
    [SerializeField] private float baseDrillImprovement; // Новая переменная
    private bool isOverheated = false;
    private Color originalDrillColor;

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
        originalDrillColor = drillSpriteRenderer.color;
        baseDrillImprovement = currentDrillImprovement; // Сохраняем базовое значение
    }

    public IEnumerator DrillDown()
    {
        while (true)
        {
            MovePlatformDown();
            //CoolingSystem.instance.UpdateCoolingVolume(0.1f);
            yield return new WaitForSeconds(0.023f);
        }
    }

    public void MovePlatformDown()
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
            CameraShake.instance?.ShakeCamera(0.01f, true);
            GameManager.instance.MiningStage();
            DroneController.instance.isActive = true;
            StartCoroutine(HardDrill());
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("SoftRock") && GameManager.instance.isDrillingActive)
        {
            CameraShake.instance?.ShakeCamera(0.07f, true);
        }

        if (other.CompareTag("Hard") && GameManager.instance.isDrillingActive)
        {
            CameraShake.instance?.ShakeCamera(0.01f, true);
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

        DroneHUD.instance.TimeUpdate(-1f);
        drillSpeed = originalDrillSpeed;
        CameraShake.instance?.ShakeCamera(0f, false);
        GameManager.instance.DrillingStage();
        DroneController.instance.isActive = false;
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

        currentDrillIndex = drillIndex;
        Drill selectedDrill = drillPool[drillIndex];
        baseDrillImprovement = selectedDrill.improvementFactor; // Обновляем базовое значение
        currentDrillImprovement = isOverheated ? 
            baseDrillImprovement * overheatDrillMultiplier : 
            baseDrillImprovement;
        
        
        if (drillSpriteRenderer != null)
        {
            drillSpriteRenderer.sprite = selectedDrill.drillSprite;
        }

        Debug.Log($"Выбрана дрель: {selectedDrill.drillName}, Коэффициент улучшения: {currentDrillImprovement}");
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
        currentDrillImprovement *= overheatDrillMultiplier;
        
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
        
        Debug.Log("Перегрев! Мощность снижена до 10%");
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
    }
}