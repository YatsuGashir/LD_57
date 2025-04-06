using UnityEngine;

[System.Serializable]
public class TurretUpgrade
{
    public string upgradeName;
    public float fireRateMultiplier;
    public float bulletSpeedMultiplier;
    public Sprite upgradeSprite;

    public TurretUpgrade(string name, float fireRateMult, float bulletSpeedMult, Sprite sprite)
    {
        upgradeName = name;
        fireRateMultiplier = fireRateMult;
        bulletSpeedMultiplier = bulletSpeedMult;
        upgradeSprite = sprite;
    }
}