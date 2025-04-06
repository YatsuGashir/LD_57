using UnityEngine;

[System.Serializable]
public class CoolingSystemUpgrade
{
    public string upgradeName;       // Название улучшения
    public float refillSpeedMultiplier; // Множитель скорости пополнения охлаждающего вещества
    public float drainSpeedMultiplier;  // Множитель скорости расхода охлаждающего вещества
    public Sprite upgradeSprite;     // Изображение для улучшения

    public CoolingSystemUpgrade(string name, float refillMultiplier, float drainMultiplier, Sprite sprite)
    {
        upgradeName = name;
        refillSpeedMultiplier = refillMultiplier;
        drainSpeedMultiplier = drainMultiplier;
        upgradeSprite = sprite;
    }
}