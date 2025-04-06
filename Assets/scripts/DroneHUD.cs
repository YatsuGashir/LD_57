using TMPro;
using UnityEngine;

public class DroneHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ironText; // Используем TextMeshProUGUI вместо GameObject
    [SerializeField] private TextMeshProUGUI timeText;
    
    public static DroneHUD instance;
    public float ironSum;

    private void Awake()
    {
        instance = this;
    }

    public void ResourcesUpdate() // Исправлено опечатку в названии метода
    {
        if (ironText != null)
        {
            ironText.text = "Iron: " + ironSum.ToString("F1"); // F1 - формат с 1 десятичным знаком
        }
    }

    public void TimeUpdate(float time)
    {
        timeText.text = "Time: " + time;
        if(time <= 0) timeText.text = " ";
    }
}
