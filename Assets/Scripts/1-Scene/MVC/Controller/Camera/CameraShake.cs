using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 0.5f; // 震动持续时间
    public float magnitude = 0.1f; // 震动幅度
    public float rotationMagnitude = 0.2f; // 旋转幅度

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float shakeTimer;
    private bool isShaking = false;

    private void Awake()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        if (!isShaking)
        {
            return;
        }

        if (shakeTimer > 0)
        {
            transform.localPosition = originalPosition + Random.insideUnitSphere * magnitude;
            transform.localRotation = new Quaternion(
                originalRotation.x + Random.Range(-rotationMagnitude, rotationMagnitude) * 0.1f,
                originalRotation.y + Random.Range(-rotationMagnitude, rotationMagnitude) * 0.1f,
                originalRotation.z + Random.Range(-rotationMagnitude, rotationMagnitude) * 0.1f,
                originalRotation.w + Random.Range(-rotationMagnitude, rotationMagnitude) * 0.1f
            );

            shakeTimer -= Time.deltaTime;
        }
        else
        {
            StopShaking();
        }
    }

    public void StartShaking(
        float durationOverride = -1f,
        float magnitudeOverride = -1f,
        float rotationOverride = -1f
    )
    {
        if (isShaking)
        {
            StopShaking();
        }

        duration = durationOverride > 0 ? durationOverride : duration;
        magnitude = magnitudeOverride > 0 ? magnitudeOverride : magnitude;
        rotationMagnitude = rotationOverride > 0 ? rotationOverride : rotationMagnitude;

        shakeTimer = duration;
        isShaking = true;
    }

    private void StopShaking()
    {
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        isShaking = false;
    }
}
