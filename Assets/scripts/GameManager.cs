using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject player;

    private float smoothSpeed = 0.125f;
    private GameObject trackingObj;
    public bool isDrillingActive { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Запускаем корутину один раз при старте
        StartCoroutine(DrillingProcess());
        DrillingStage(); // Начинаем в режиме бурения
    }

    private IEnumerator DrillingProcess()
    {
        while (true)
        {
            //if (isDrillingActive)
            //{
                DrillController.Instance.MovePlatformDown();
                CoolingSystem.instance.DrainCooling(0.1f); // Используем новый метод
            //}
            yield return new WaitForSeconds(0.023f);
        }
    }

    public void DrillingStage()
    {
        isDrillingActive = true;
        trackingObj = platform;
        CameraShake.instance.ShakeCamera(0.01f, true);
    }

    public void MiningStage()
    {
        isDrillingActive = false;
        trackingObj = player;
        CameraShake.instance.ShakeCamera(0f, false);
    }

    private void FixedUpdate()
    {
        if (trackingObj != null)
        {
            Vector3 targetPosition = new Vector3(
                trackingObj.transform.position.x,
                trackingObj.transform.position.y,
                cam.transform.position.z
            );
            Vector3 smoothed = Vector3.Lerp(cam.transform.position, targetPosition, smoothSpeed);
            cam.transform.position = smoothed + CameraShake.instance.ShakeOffset;
        }
    }
}