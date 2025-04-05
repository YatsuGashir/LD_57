using UnityEngine;
using UnityEngine.Tilemaps;

public class DroneController : MonoBehaviour
{
    public static DroneController instance;
    
    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject box;
    
    [Header("Настройки добычи")]
    [SerializeField] private float miningRange = 1.5f; // Дистанция для начала добычи
    [SerializeField] private float miningTime = 0.5f; // Время добычи одного тайла
    [SerializeField] private float shakeIntensity = 0.1f; // Сила тряски
    
    [Header("Настройки ввода")]
    [SerializeField] private float modeSwitchCooldown = 0.5f;
    
    private Vector3 targetPosition;
    public bool isActive = false;
    private float lastSwitchTime = -1f;
    private GameObject currentOreTarget;
    private float miningTimer;
    private Vector3 originalOrePosition;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1) && Time.time - lastSwitchTime >= modeSwitchCooldown)
        {
            lastSwitchTime = Time.time;
            
            if (isActive)
            {
                GameManager.instance.DrillingStage();
                isActive = false;
            }
            else
            {
                GameManager.instance.MiningStage();
                isActive = true;
            }
        }

        if (isActive)
        {
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorPosition.z = 0f;
            targetPosition = cursorPosition;
            
            // Поиск ближайшей руды
            FindNearestOre();
            
            // Если есть цель для добычи и в радиусе
            if (currentOreTarget != null && 
                Vector3.Distance(transform.position, currentOreTarget.transform.position) <= miningRange)
            {
                MineOre();
            }
            else
            {
                miningTimer = 0f;
            }
        }
        else
        {
            Vector3 cursorPosition = box.transform.position;
            cursorPosition.z = 0f;
            targetPosition = cursorPosition;
        }

        MoveTowardsTarget();
    }

    private void FindNearestOre()
    {
        Collider2D[] ores = Physics2D.OverlapCircleAll(transform.position, miningRange * 2f);
        float closestDistance = Mathf.Infinity;
        GameObject closestOre = null;

        foreach (Collider2D ore in ores)
        {
            if (ore.CompareTag("Ore"))
            {
                float distance = Vector3.Distance(transform.position, ore.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestOre = ore.gameObject;
                }
            }
        }

        currentOreTarget = closestOre;
        if (currentOreTarget != null)
        {
            originalOrePosition = currentOreTarget.transform.position;
        }
    }

    private void MineOre()
    {
        miningTimer += Time.deltaTime;
        
        // Эффект тряски
        if (currentOreTarget != null)
        {
            float shakeX = originalOrePosition.x + Random.Range(-shakeIntensity, shakeIntensity);
            float shakeY = originalOrePosition.y + Random.Range(-shakeIntensity, shakeIntensity);
            currentOreTarget.transform.position = new Vector3(shakeX, shakeY, originalOrePosition.z);
        }

        // Когда время добычи истекло
        if (miningTimer >= miningTime && currentOreTarget != null)
        {
            Destroy(currentOreTarget);
            currentOreTarget = null;
            miningTimer = 0f;
            
            // Здесь можно добавить эффекты/звуки/награду за добычу
        }
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, miningRange);
    }
}