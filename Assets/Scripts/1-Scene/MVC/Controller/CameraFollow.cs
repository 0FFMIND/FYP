using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField]
    private CinemachineVirtualCamera vcam;
    private Coroutine zoomCo;

    [Header("Shake Settings")]
    public float duration = 0.5f; // 震动持续时间
    public float magnitude = 0.1f; // 震动幅度
    public float rotationMagnitude = 0.05f; // 旋转幅度

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float shakeTimer;
    private bool isShaking = false;

    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        vcam.Follow = player.transform;
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    public void ZoomOrthoBy(float delta, float duration)
    {
        float current = vcam.m_Lens.OrthographicSize;
        float target = current - delta;
        ZoomOrtho(target, duration);
    }

    public void ZoomOrtho(float targetSize, float duration)
    {
        if (zoomCo != null)
            StopCoroutine(zoomCo);
        targetSize = Mathf.Max(0.01f, targetSize);

        if (duration <= 0f)
        {
            var lens = vcam.m_Lens;
            lens.OrthographicSize = targetSize;
            vcam.m_Lens = lens;
            return;
        }

        zoomCo = StartCoroutine(
            LerpFloat(
                () => vcam.m_Lens.OrthographicSize,
                v =>
                {
                    var lens = vcam.m_Lens;
                    lens.OrthographicSize = v;
                    vcam.m_Lens = lens;
                },
                targetSize,
                duration
            )
        );
    }

    // 用 AnimationCurve 做缓动的通用插值
    private IEnumerator LerpFloat(
        System.Func<float> getter,
        System.Action<float> setter,
        float target,
        float duration
    )
    {
        float start = getter();
        float t = 0f;

        // 防空：没配曲线就用线性
        var curve = zoomCurve != null ? zoomCurve : AnimationCurve.Linear(0, 0, 1, 1);

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            float eased = curve.Evaluate(k);
            setter(Mathf.Lerp(start, target, eased));
            yield return null;
        }

        setter(target);
        zoomCo = null;
    }

    void Update()
    {
        if (!isShaking)
            return;

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
            StopShaking();

        duration = durationOverride > 0 ? durationOverride : duration;
        magnitude = magnitudeOverride > 0 ? magnitudeOverride : magnitude;
        rotationMagnitude = rotationOverride > 0 ? rotationOverride : rotationMagnitude;

        shakeTimer = duration;
        isShaking = true;
    }

    public void StopShaking()
    {
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        isShaking = false;
    }
}
