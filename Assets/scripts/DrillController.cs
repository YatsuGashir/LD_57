using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrillController : MonoBehaviour
{
    public static DrillController Instance;

    [SerializeField] private GameObject platform;
    [SerializeField] private float originalDrillSpeed = 0.005f;
    private float drillSpeed;
    private SpriteRenderer sprite;

    // Пул дрелей, который можно редактировать через инспектор
    public List<Drill> drillPool;

    // Коэффициент улучшения текущей дрели
    public float currentDrillImprovement = 1f;

    // Спрайт, который будет отображаться для текущей дрели
    public SpriteRenderer drillSpriteRenderer;

    // Tilemap для удаления тайлов
    [SerializeField] private Tilemap tilemap;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SelectDrill(0);
        drillSpeed = originalDrillSpeed;

        //StartCoroutine(DrillDown());

    }

    public IEnumerator DrillDown()
    {
        while (true) // Эта корутина будет выполняться бесконечно
        {
            MovePlatformDown();
            CoolingSystem.instance.UpdateCoolingVolume(0.01f);
            yield return new WaitForSeconds(0.023f);
        }
    }
    
  

    private void MovePlatformDown()
    {
        var vector3 = platform.transform.position;
        vector3.y = vector3.y - drillSpeed * currentDrillImprovement;
        platform.transform.position = vector3;
        
        CheckAndRemoveTilesUnderPlatform();
    }
    
    private void CheckAndRemoveTilesUnderPlatform()
    {
        BoxCollider2D platformCollider = platform.GetComponent<BoxCollider2D>();
        if (platformCollider == null || tilemap == null) return;

        Bounds bounds = platformCollider.bounds;
        Vector2 bottomLeft = new Vector2(bounds.min.x, bounds.min.y - 0.1f);
        Vector2 bottomRight = new Vector2(bounds.max.x, bounds.min.y - 0.1f);

        Vector3Int leftCell = tilemap.WorldToCell(bottomLeft);
        Vector3Int rightCell = tilemap.WorldToCell(bottomRight);

        for (int x = leftCell.x; x <= rightCell.x; x++)
        {
            Vector3Int cell = new Vector3Int(x, leftCell.y, leftCell.z);
            if (tilemap.HasTile(cell))
            {
                tilemap.SetTile(cell, null);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SoftRock"))
        {
            Debug.Log("Бур замедлен из-за земли");
            drillSpeed *= 0.5f;
            CameraShake.instance.ShakeCamera(0.07f, true);
        }

       
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        drillSpeed = originalDrillSpeed;
        CameraShake.instance.ShakeCamera(0.01f, true);
    }



    // Выбор дрели для использования
    public void SelectDrill(int drillIndex)
    {
        Drill selectedDrill = drillPool[drillIndex];
        currentDrillImprovement = selectedDrill.improvementFactor;
        drillSpriteRenderer.sprite = selectedDrill.drillSprite;
        Debug.Log($"Выбрана дрель: {selectedDrill.drillName}, Коэффициент улучшения: {currentDrillImprovement}");
    }
}
