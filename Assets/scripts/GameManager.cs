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
    private bool isDrillingActive = false;

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
            if (isDrillingActive)
            {
                DrillController.Instance.MovePlatformDown();
                CoolingSystem.instance.UpdateCoolingVolume(0.01f);
            }

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