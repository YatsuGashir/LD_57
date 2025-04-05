using System;
using UnityEngine;

public class CoolingSystem : MonoBehaviour
{
    public static CoolingSystem instance;
    
    [SerializeField] private GameObject coolingSystem;
    private float coolingVolumeStart=100f;
    public float coolingVolume;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        coolingSystem.gameObject.GetComponent<PlatformBar>().SetMaxBar(coolingVolumeStart);
        coolingVolume= coolingVolumeStart;
        
    }

    public void UpdateCoolingVolume(float volume)
    {
        coolingVolume -= volume;
        coolingSystem.gameObject.GetComponent<PlatformBar>().SetBar(coolingVolume);
    }
    
}
