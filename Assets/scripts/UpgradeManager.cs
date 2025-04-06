using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("UI элементы")]
    [SerializeField] private TextMeshProUGUI oreCountText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private Button upgradeButton;

    private int oreCount = 0;
    private int upgradeCost = 2; // Примерная стоимость улучшения

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // Обновляем UI с количеством руды
        oreCountText.text = "Руда: " + oreCount;

        // Обновляем стоимость улучшения
        upgradeCostText.text = "Стоимость улучшения: " + upgradeCost;

        // Включаем или выключаем кнопку в зависимости от количества руды
        upgradeButton.interactable = oreCount >= upgradeCost;
    }

    public void AddOre(int amount)
    {
        oreCount += amount;
    }

    public void UpgradeDrill()
    {
        if (oreCount >= upgradeCost)
        {
            oreCount -= upgradeCost; // Снимаем стоимость улучшения
            DrillController.Instance.SelectDrill(1); // Применяем улучшение

            // Можете добавить сюда дополнительные улучшения
        }
    }
}