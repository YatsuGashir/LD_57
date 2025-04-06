using UnityEngine;

public class CoolingSystem : MonoBehaviour
{
    public static CoolingSystem instance;
    
    [SerializeField] private GameObject coolingSystem;
    [SerializeField] private float coolingVolumeStart = 100f;
    [SerializeField] private float refillSpeed = 20f;
    public float coolingVolume { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        coolingSystem.GetComponent<PlatformBar>().SetMaxBar(coolingVolumeStart);
        coolingVolume = coolingVolumeStart;
    }

    // Измененный метод для расхода
    public void DrainCooling(float amount)
    {
        coolingVolume = Mathf.Clamp(coolingVolume - amount, 0, coolingVolumeStart);
        coolingSystem.GetComponent<PlatformBar>().SetBar(coolingVolume);
        DrillController.Instance.CheckOverheat(coolingVolume);
    }

    // Метод для пополнения
    public void RefillCooling(float amount)
    {
        coolingVolume = Mathf.Clamp(coolingVolume + amount, 0, coolingVolumeStart);
        coolingSystem.GetComponent<PlatformBar>().SetBar(coolingVolume);
    }
}