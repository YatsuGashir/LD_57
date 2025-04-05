using System;
using UnityEngine;
using UnityEngine.UI;

public class PlatformBar : MonoBehaviour
{

    
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;



    public void SetMaxBar(float volume)
    {
        healthSlider.maxValue = volume;
        healthSlider.value = volume;
        
        fill.color = gradient.Evaluate(1f);
    }
    public void SetBar(float volume)
    {
        healthSlider.value = volume;
        
        fill.color = gradient.Evaluate(healthSlider.normalizedValue);
    }


}