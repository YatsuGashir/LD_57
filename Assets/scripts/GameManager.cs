using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera mainCamera;

    private float smoothSpeed = 0.125f;
    private GameObject trackingObj;
    public bool isDrillingActive { get; private set; }

    private Vector3 miningCamOffset = new Vector3(0f, 0f, -10f);
    private Vector3 drillingCamOffset = new Vector3(0f, 6f, -10f);

    private float miningZoom = 4f;
    private float drillingZoom = 5f;
    private float zoomSmoothSpeed = 5f;
    private float targetZoom;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

        DrillingStage();

        // Инициализация камеры и начального зума
        if (mainCamera != null)
        {
            targetZoom = drillingZoom;
            mainCamera.orthographicSize = targetZoom;
        }
    }



    public void DrillingStage()
    {
        isDrillingActive = true;
        trackingObj = platform;
        CameraShake.instance.ShakeCamera(0.01f, true);
        SetCameraMode(true);
    }

    public void MiningStage()
    {
        isDrillingActive = false;
        trackingObj = player;
        CameraShake.instance.ShakeCamera(0f, false);
        SetCameraMode(false);
    }

    private void SetCameraMode(bool drillingMode)
    {
        if (mainCamera == null) return;

        targetZoom = drillingMode ? drillingZoom : miningZoom;
    }

    private void FixedUpdate()
    {
        if (trackingObj != null && mainCamera != null)
        {
            Vector3 targetPosition = new Vector3(
                trackingObj.transform.position.x,
                trackingObj.transform.position.y,
                0f
            );

            Vector3 offset = isDrillingActive ? drillingCamOffset : miningCamOffset;
            Vector3 baseTarget = targetPosition + offset;
            Vector3 smoothed = Vector3.Lerp(mainCamera.transform.position, baseTarget, smoothSpeed);
            mainCamera.transform.position = smoothed + CameraShake.instance.ShakeOffset;

            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, Time.fixedDeltaTime * zoomSmoothSpeed);
        }
    }
}
