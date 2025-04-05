using UnityEngine;
[System.Serializable] 
public class Drill
{
    public string drillName;        // Имя дрели
    public float improvementFactor; // Коэффициент улучшения, например, 1.2f означает улучшение на 20%
    public Sprite drillSprite;

    public Drill(string name, float factor, Sprite sprite)
    {
        drillName = name;
        improvementFactor = factor;
        drillSprite = sprite;
    }
}
