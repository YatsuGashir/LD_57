using UnityEngine;

public class CoolingSystem : MonoBehaviour
{
    public static CoolingSystem instance;

    [Header("UI элементы")]
    [SerializeField] private GameObject coolingSystemUI; // Ссылка на UI охладителя
    [SerializeField] private float coolingVolumeStart = 100f;
    [SerializeField] private float baseRefillSpeed = 20f;  // Базовая скорость пополнения
    [SerializeField] private float baseDrainSpeed = 0.1f;   // Базовая скорость расхода
    [SerializeField] private SpriteRenderer coolingSystemSprite; // Сам спрайт объекта охладителя


    public float coolingVolume { get; private set; }

    [Header("Система улучшений охладителя")]
    public CoolingSystemUpgrade[] upgrades; // Массив улучшений
    private int currentUpgradeIndex = 0; // Индекс текущего улучшения
    private float refillSpeed;  // Текущая скорость пополнения
    private float drainSpeed;   // Текущая скорость расхода
    private bool firstReload = false;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        coolingSystemUI.GetComponent<PlatformBar>().SetMaxBar(coolingVolumeStart); // Устанавливаем максимальное значение охлаждения
        coolingVolume = 0;
        coolingSystemUI.GetComponent<PlatformBar>().SetBar(coolingVolume);  // Обновляем UI охлаждения
        ApplyUpgrade(currentUpgradeIndex); // Применяем первое улучшение
    }

    // Применяем улучшение
    private void ApplyUpgrade(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length) return;

        CoolingSystemUpgrade upgrade = upgrades[upgradeIndex];
        refillSpeed = baseRefillSpeed * upgrade.refillSpeedMultiplier;
        drainSpeed = baseDrainSpeed * upgrade.drainSpeedMultiplier;

        if (coolingSystemSprite != null && upgrade.upgradeSprite != null)
        {
            coolingSystemSprite.sprite = upgrade.upgradeSprite;
        }

        Debug.Log($"Охладитель улучшен: {upgrade.upgradeName}");
    }


    // Изменённый метод для расхода охлаждающей жидкости
    public void DrainCooling(float amount)
    {
        coolingVolume = Mathf.Clamp(coolingVolume - (amount * drainSpeed), 0, coolingVolumeStart);
        coolingSystemUI.GetComponent<PlatformBar>().SetBar(coolingVolume);  // Обновляем UI охлаждения
        DrillController.Instance.CheckOverheat(coolingVolume);
    }

    // Метод для пополнения охлаждающей жидкости
    public void RefillCooling(float amount)
    {
        coolingVolume = Mathf.Clamp(coolingVolume + (amount * refillSpeed), 0, coolingVolumeStart);
        coolingSystemUI.GetComponent<PlatformBar>().SetBar(coolingVolume);  // Обновляем UI охлаждения
        if (!firstReload)
        {
            if (coolingVolume == coolingVolumeStart)
            {
                TutorialManager.instance.firstCoolR();
                firstReload = true;
            }
        }
    }

    // Метод для смены улучшения
    public void UpgradeCoolingSystem(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length) return;
        currentUpgradeIndex = upgradeIndex;
        ApplyUpgrade(upgradeIndex);  // Применяем новое улучшение
    }
}
