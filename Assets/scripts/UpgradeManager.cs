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
    
    [Header("Улучшение платформы")]
    [SerializeField] private TextMeshProUGUI platformUpgradeCostText;
    [SerializeField] private Button platformUpgradeButton;
    private int platformUpgradeCost = 6;
    private int currentPlatformUpgradeIndex = 0;

    [SerializeField] private PlatformHealth platformHealth;
    [Header("Система диалогов")]
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] Sprite portrait1;
    
    private Sprite portrait2;
    private int currentDrill=0;


    private int oreCount = 0;

    private void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        oreCountText.text = "Ore: " + oreCount;

        // Бур
        if (currentDrill >= 3)
        {
            upgradeCostText.text = "<color=red>Level Max</color>";
            upgradeButton.interactable = false;
        }
        else
        {
            upgradeCostText.text = "Upgrade drill: " + upgradeCost;
            upgradeButton.interactable = oreCount >= upgradeCost;
        }

        // Охладитель
        if (currentCoolingUpgradeIndex >= 3)
        {
            coolingUpgradeCostText.text = "<color=red>Level Max</color>";
            coolingUpgradeButton.interactable = false;
        }
        else
        {
            coolingUpgradeCostText.text = "Upgrade cooling system: " + coolingUpgradeCost;
            coolingUpgradeButton.interactable =
                oreCount >= coolingUpgradeCost &&
                currentCoolingUpgradeIndex + 1 < CoolingSystem.instance.upgrades.Length;
        }

        // Турель
        if (currentTurretUpgradeIndex >= 3)
        {
            turretUpgradeCostText.text = "<color=red>Level Max</color>";
            turretUpgradeButton.interactable = false;
        }
        else
        {
            turretUpgradeCostText.text = "Upgrade turret: " + turretUpgradeCost;
            turretUpgradeButton.interactable =
                oreCount >= turretUpgradeCost &&
                currentTurretUpgradeIndex + 1 < TurretController.instance.upgrades.Length;
        }

        // Платформа
        if (currentPlatformUpgradeIndex >= 3)
        {
            platformUpgradeCostText.text = "<color=red>Level Max</color>";
            platformUpgradeButton.interactable = false;
        }
        else
        {
            platformUpgradeCostText.text = "Upgrade platform: " + platformUpgradeCost;
            platformUpgradeButton.interactable =
                oreCount >= platformUpgradeCost &&
                currentPlatformUpgradeIndex + 1 < platformHealth.GetUpgradeCount();
        }
    }

    public void AddOre(int amount)
    {
        oreCount += amount;
    }

    public void UpgradeDrill()
    {
        if (currentDrill >= 3) return;
        if (oreCount >= upgradeCost)
        {
            currentDrill++;
            AudioManager.instance.Play("upgrade");
            oreCount -= upgradeCost;
            upgradeCost += 2;
            DrillController.Instance.SelectDrill(currentDrill);
        }
    }

    public void UpgradeCoolingSystem()
    {
        if (currentCoolingUpgradeIndex >= 3) return;
        if (oreCount >= coolingUpgradeCost &&
            currentCoolingUpgradeIndex + 1 < CoolingSystem.instance.upgrades.Length)
        {
            AudioManager.instance.Play("upgrade");
            oreCount -= coolingUpgradeCost;
            currentCoolingUpgradeIndex++;
            CoolingSystem.instance.UpgradeCoolingSystem(currentCoolingUpgradeIndex);
            coolingUpgradeCost += 2;
        }
    }

    public void UpgradeTurret()
    {
        if (currentTurretUpgradeIndex >= 3) return;
        if (oreCount >= turretUpgradeCost &&
            currentTurretUpgradeIndex + 1 < TurretController.instance.upgrades.Length)
        {
            AudioManager.instance.Play("upgrade");
            oreCount -= turretUpgradeCost;
            currentTurretUpgradeIndex++;
            TurretController.instance.UpgradeTurret(currentTurretUpgradeIndex);
            turretUpgradeCost += 3;
        }
    }
    
    public void UpgradePlatform()
    {
        if (currentPlatformUpgradeIndex >= 3) return;
        
        if (oreCount >= platformUpgradeCost &&
            currentPlatformUpgradeIndex + 1 < platformHealth.GetUpgradeCount())
        {
            AudioManager.instance.Play("upgrade");
            oreCount -= platformUpgradeCost;
            currentPlatformUpgradeIndex++;
            platformHealth.ApplyUpgrade(currentPlatformUpgradeIndex);
            platformUpgradeCost += 4;

        }
    }
}
