using UnityEngine;

public class CoolingSystem : MonoBehaviour
{
    public static CoolingSystem instance;
    
    [Header("Настройки")]
    [SerializeField] private GameObject coolingSystem;
    [SerializeField] private float coolingVolumeStart = 100f;
    [SerializeField] private float rechargeRate = 10f; // Скорость перезарядки
    [SerializeField] private float rechargeDistance = 2f; // Дистанция для начала зарядки
    
    [Header("Текущее состояние")]
    public float coolingVolume;
    private bool isRecharging = false;

    private Transform playerDrone; // Ссылка на трансформ дрона

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Находим дрон по тегу (или получите ссылку другим способом)
        playerDrone = GameObject.FindGameObjectWithTag("Player").transform;
        
        coolingSystem.GetComponent<PlatformBar>().SetMaxBar(coolingVolumeStart);
        coolingVolume = coolingVolumeStart;
    }

    void Update()
    {
        CheckRechargeCondition();
        
        if (isRecharging)
        {
            RechargeCooling();
        }
    }

    public void UpdateCoolingVolume(float volume)
    {
        coolingVolume -= volume;
        coolingVolume = Mathf.Clamp(coolingVolume, 0, coolingVolumeStart);
        coolingSystem.GetComponent<PlatformBar>().SetBar(coolingVolume);
    
        // Проверка перегрева
        DrillController.Instance.CheckOverheat(coolingVolume);
    }

    private void CheckRechargeCondition()
    {
        if (playerDrone == null) return;

        // Проверяем дистанцию до дрона
        float distance = Vector3.Distance(transform.position, playerDrone.position);
        bool shouldRecharge = distance <= rechargeDistance && coolingVolume < coolingVolumeStart;

        // Если состояние изменилось
        if (shouldRecharge != isRecharging)
        {
            isRecharging = shouldRecharge;
            Debug.Log(isRecharging ? "Началась зарядка" : "Зарядка остановлена");
        }
    }

    private void RechargeCooling()
    {
        coolingVolume += rechargeRate * Time.deltaTime;
        coolingVolume = Mathf.Min(coolingVolume, coolingVolumeStart);
        coolingSystem.GetComponent<PlatformBar>().SetBar(coolingVolume);
        
        if (coolingVolume >= coolingVolumeStart)
        {
            isRecharging = false;
        }
    }

    // Визуализация радиуса зарядки в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rechargeDistance);
    }
}