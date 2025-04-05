using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject player;
    
    private float smoothSpeed = 0.125f;  // Скорость плавного следования камеры
    private GameObject trackingObj;
    private Coroutine drillingCoroutine; // Храним ссылку на корутину


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

        // Начинаем с бурения
        DrillingStage();
        //MiningStage();
    }

    public void DrillingStage()
    {
        // Останавливаем предыдущую корутину, если она есть
        if (drillingCoroutine != null)
        {
            StopCoroutine(drillingCoroutine);
        }
        
        trackingObj = platform;
        // Стартуем бурение
        drillingCoroutine = StartCoroutine(DrillController.Instance.DrillDown());

        // Включаем дрожание камеры
        CameraShake.instance.ShakeCamera(0.01f, true);  // Пример значения для дрожания
    }
    
    public void MiningStage()
    {
        /*// Останавливаем корутину, если она есть
        if (drillingCoroutine != null)
        {
            StopCoroutine(drillingCoroutine);
            drillingCoroutine = null;
        }*/
        
        trackingObj = player;
        // Останавливаем дрожание камеры
        CameraShake.instance.ShakeCamera(0f, false);  // Прекращаем дрожание, задавая нулевую амплитуду
    }

    private void FixedUpdate()
    {
        Vector3 targetPosition = new Vector3(
            trackingObj.transform.position.x,
            trackingObj.transform.position.y,
            cam.transform.position.z
        );

        // Плавное следование
        Vector3 smoothed = Vector3.Lerp(cam.transform.position, targetPosition, smoothSpeed);

        // Прибавляем дрожание
        cam.transform.position = smoothed + CameraShake.instance.ShakeOffset;
    }

}