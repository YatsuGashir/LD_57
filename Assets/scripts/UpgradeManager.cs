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
    private int platformUpgradeCost = 5;
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
        
        // Платформа
        platformUpgradeCostText.text = "Улучшение платформы: " + platformUpgradeCost;
        platformUpgradeButton.interactable =
            oreCount >= platformUpgradeCost &&
            currentPlatformUpgradeIndex + 1 < platformHealth.GetUpgradeCount();

    }

    public void AddOre(int amount)
    {
        oreCount += amount;
    }

    public void UpgradeDrill()
    {
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
        
        if (oreCount >= platformUpgradeCost &&
            currentPlatformUpgradeIndex + 1 < platformHealth.GetUpgradeCount())
        {
            AudioManager.instance.Play("upgrade");
            oreCount -= platformUpgradeCost;
            currentPlatformUpgradeIndex++;
            platformHealth.ApplyUpgrade(currentPlatformUpgradeIndex);
            platformUpgradeCost += 4;
            DialogueLine[] lines = new DialogueLine[]
            {
                new DialogueLine { text = "Ты это чувствуешь?..", portrait = portrait1 },
                new DialogueLine { text = "Будь осторожен. Мы не знаем, что внизу.", portrait = portrait1 }
            };

            dialogueManager.StartDialogue(lines);
        }
    }
}
