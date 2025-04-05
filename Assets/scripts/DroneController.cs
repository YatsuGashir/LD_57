using UnityEngine;
using UnityEngine.Tilemaps;

public class DroneController : MonoBehaviour
{
    public static DroneController instance;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject box;
    
    [Header("Mining Settings")]
    [SerializeField] private float miningRange = 1.5f;
    [SerializeField] private float miningTime = 0.5f;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private Tilemap oreTilemap;
    [SerializeField] private TileBase oreTile;
    
    [Header("Input Settings")]
    [SerializeField] private float modeSwitchCooldown = 0.5f;
    
    private Vector3 targetPosition;
    public bool isActive = false;
    private float lastSwitchTime = -1f;
    private Vector3Int currentMiningCell;
    private float miningTimer;
    private Vector3 originalTilePosition;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        HandleModeSwitch();
        HandleMovement();
        
        if(isActive)
        {
            FindOreTile();
            MineOre();
        }
    }

    private void HandleModeSwitch()
    {
        if (Input.GetMouseButton(1) && Time.time - lastSwitchTime >= modeSwitchCooldown)
        {
            lastSwitchTime = Time.time;
            isActive = !isActive;
            
            if(isActive) GameManager.instance.MiningStage();
            else GameManager.instance.DrillingStage();
        }
    }

    private void HandleMovement()
    {
        targetPosition = isActive ? 
            Camera.main.ScreenToWorldPoint(Input.mousePosition) : 
            box.transform.position;
        
        targetPosition.z = 0;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void FindOreTile()
    {
        Vector3Int cellPosition = oreTilemap.WorldToCell(transform.position);
        
        // Проверяем 3x3 область вокруг дрона
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                Vector3Int checkCell = cellPosition + new Vector3Int(x, y, 0);
                
                if(oreTilemap.GetTile(checkCell) == oreTile && 
                   Vector3.Distance(transform.position, oreTilemap.GetCellCenterWorld(checkCell)) <= miningRange)
                {
                    currentMiningCell = checkCell;
                    originalTilePosition = oreTilemap.GetCellCenterWorld(checkCell);
                    return;
                }
            }
        }
        
        // Если ничего не нашли
        currentMiningCell = new Vector3Int(-1000, -1000, 0);
    }

    private void MineOre()
    {
        if(currentMiningCell.x == -1000) return;
        
        miningTimer += Time.deltaTime;
        
        // Эффект тряски
        Vector3 shakeOffset = new Vector3(
            Random.Range(-shakeIntensity, shakeIntensity),
            Random.Range(-shakeIntensity, shakeIntensity),
            0);
        
        oreTilemap.transform.position = originalTilePosition + shakeOffset;
        
        if(miningTimer >= miningTime)
        {
            oreTilemap.SetTile(currentMiningCell, null);
            miningTimer = 0f;
            oreTilemap.transform.position = originalTilePosition;
            
            // Здесь можно добавить:
            // 1. Эффект разрушения
            // 2. Добавление ресурсов
            // 3. Звук добычи
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, miningRange);
    }
}