using TMPro;
using UnityEngine;

public class DroneHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ironText; 
    [SerializeField] private TextMeshProUGUI timeText; 
    
    public static DroneHUD instance;
    public float ironSum;

    private void Awake()
    {
        instance = this;
    }

    public void ResourcesUpdate() // Исправлено опечатку в названии метода
    { 
        ironText.text = "Iron: " + ironSum.ToString();
    }

    public void TimeUpdate(float time)
    {
        timeText.text = "Time: " + time.ToString();
        if (time <= 0) timeText.text = " ";
    }
}