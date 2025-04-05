using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    private float shakeMagnitude = 0f;
    private bool isShake = false;
    private float decreaseFactor = 1f;
    private Vector3 shakeOffset = Vector3.zero;

    public Vector3 ShakeOffset => shakeOffset;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (isShake)
        {
            shakeOffset = (Vector3)(Random.insideUnitCircle * shakeMagnitude);

        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    public void ShakeCamera(float magnitude, bool shake)
    {
        shakeMagnitude = magnitude;
        isShake = shake;
    }
}