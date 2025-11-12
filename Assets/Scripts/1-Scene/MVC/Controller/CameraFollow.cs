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

    private Transform player;
    private Transform manualAnchor;
    private Coroutine panCo;

    private void Awake()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        player = p != null ? p.transform : null;

        vcam.Follow = player.transform;
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }
    public void DetachCamera()
    {
        if (manualAnchor == null)
        {
            manualAnchor = new GameObject("CamManualAnchor").transform;
            manualAnchor.position = vcam.transform.position; // 与当前机位对齐
            manualAnchor.rotation = vcam.transform.rotation;
        }
        vcam.Follow = manualAnchor;
        // 若你同时用了 LookAt（3D），也建议：
        // vcam.LookAt = null;
    }

    // === 新增：重新跟随玩家 ===
    public void ReattachToPlayer()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            player = p != null ? p.transform : null;
        }
        if (player != null) vcam.Follow = player;
    }

    // === 新增：对外方法，按给定 Vector2 世界坐标平移摄像机（保持当前 z）===
    // 用于过场、展示场景细节等
    public void PanTo(Vector2 xy, float duration)
    {
        DetachCamera(); // 确保已脱离人物，交给锚点驱动

        if (panCo != null) StopCoroutine(panCo);
        var start = (manualAnchor != null ? manualAnchor.position : vcam.transform.position);
        var target = new Vector3(xy.x, xy.y, start.z); // 保持 z 不变（正交相机）

        panCo = StartCoroutine(LerpVector3(
            () => manualAnchor.position,
            v => manualAnchor.position = v,
            target,
            duration
        ));
    }

    public void PanToY(float y, float duration)
    {
        DetachCamera(); // 确保已脱离人物，交给锚点驱动

        if (panCo != null) StopCoroutine(panCo);
        var start = (manualAnchor != null ? manualAnchor.position : vcam.transform.position);
        var target = new Vector3(start.x, y, start.z); // 保持 z 不变（正交相机）

        panCo = StartCoroutine(LerpVector3(
            () => manualAnchor.position,
            v => manualAnchor.position = v,
            target,
            duration
        ));
    }

    // === 新增：通用 Vector3 插值（与现有 LerpFloat 一致的曲线节奏）===
    private IEnumerator LerpVector3(
        System.Func<Vector3> getter,
        System.Action<Vector3> setter,
        Vector3 target,
        float duration
    )
    {
        if (duration <= 0f)
        {
            setter(target);
            yield break;
        }

        Vector3 start = getter();
        float t = 0f;
        var curve = zoomCurve != null ? zoomCurve : AnimationCurve.Linear(0, 0, 1, 1);

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            float eased = curve.Evaluate(k);
            setter(Vector3.Lerp(start, target, eased));
            yield return null;
        }
        setter(target);
        panCo = null;
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
