using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject player;
    
    private float smoothSpeed = 0.125f;  // Скорость плавного следования камеры
    private GameObject trackingObj;

    private void Start()
    {

        // Начинаем с бурения
        DrillingStage();
    }

    private void DrillingStage()
    {
        trackingObj = platform;
        // Стартуем бурение
        StartCoroutine(DrillController.Instance.DrillDown());

        // Включаем дрожание камеры
        CameraShake.instance.ShakeCamera(0.01f, true);  // Пример значения для дрожания
    }
    
    private void MiningStage()
    {
        trackingObj = player;
        // Стартуем с дроном
        StartCoroutine(DrillController.Instance.DrillDown());

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