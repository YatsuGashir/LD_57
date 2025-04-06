using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("UI элементы")]
    [SerializeField] private TextMeshProUGUI oreCountText;

    [Header("Улучшение бура")]
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private Button upgradeButton;
    private int upgradeCost = 2;

    [Header("Улучшение охлаждения")]
    [SerializeField] private TextMeshProUGUI coolingUpgradeCostText;
    [SerializeField] private Button coolingUpgradeButton;
    private int coolingUpgradeCost = 3;
    private int currentCoolingUpgradeIndex = 0;

    [Header("Улучшение турели")]
    [SerializeField] private TextMeshProUGUI turretUpgradeCostText;
    [SerializeField] private Button turretUpgradeButton;
    private int turretUpgradeCost = 4;
    private int currentTurretUpgradeIndex = 0;

    private int oreCount = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // Обновляем UI ресурсов
        oreCountText.text = "Руда: " + oreCount;

        // Бур
        upgradeCostText.text = "Стоимость улучшения: " + upgradeCost;
        upgradeButton.interactable = oreCount >= upgradeCost;

        // Охладитель
        coolingUpgradeCostText.text = "Улучшение охлаждения: " + coolingUpgradeCost;
        coolingUpgradeButton.interactable =
            oreCount >= coolingUpgradeCost &&
            currentCoolingUpgradeIndex + 1 < CoolingSystem.instance.upgrades.Length;

        // Турель
        turretUpgradeCostText.text = "Улучшение турели: " + turretUpgradeCost;
        turretUpgradeButton.interactable =
            oreCount >= turretUpgradeCost &&
            currentTurretUpgradeIndex + 1 < TurretController.instance.upgrades.Length;
    }

    public void AddOre(int amount)
    {
        oreCount += amount;
    }

    public void UpgradeDrill()
    {
        if (oreCount >= upgradeCost)
        {
            oreCount -= upgradeCost;
            DrillController.Instance.SelectDrill(1);
        }
    }

    public void UpgradeCoolingSystem()
    {
        if (oreCount >= coolingUpgradeCost &&
            currentCoolingUpgradeIndex + 1 < CoolingSystem.instance.upgrades.Length)
        {
            oreCount -= coolingUpgradeCost;
            currentCoolingUpgradeIndex++;
            CoolingSystem.instance.UpgradeCoolingSystem(currentCoolingUpgradeIndex);
            coolingUpgradeCost += 2;
        }
    }

    public void UpgradeTurret()
    {
        if (oreCount >= turretUpgradeCost &&
            currentTurretUpgradeIndex + 1 < TurretController.instance.upgrades.Length)
        {
            oreCount -= turretUpgradeCost;
            currentTurretUpgradeIndex++;
            TurretController.instance.UpgradeTurret(currentTurretUpgradeIndex);
            turretUpgradeCost += 3;
        }
    }
}
