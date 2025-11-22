using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField]
    private CinemachineVirtualCamera playerVCam;

    private Coroutine zoomCo;

    private Transform manualAnchor;
    private Coroutine panCo;

    // === 对外方法，按给定 Vector2 世界坐标平移摄像机（保持当前 z）===
    public void PanTo(Vector2 xy, float duration)
    {
        if (panCo != null)
            StopCoroutine(panCo);
        var start = (manualAnchor != null ? manualAnchor.position : playerVCam.transform.position);
        var target = new Vector3(xy.x, xy.y, start.z); // 保持 z 不变（正交相机）

        panCo = StartCoroutine(
            LerpVector3(
                () => manualAnchor.position,
                v => manualAnchor.position = v,
                target,
                duration
            )
        );
    }

    public void PanToY(float y, float duration)
    {
        if (panCo != null)
            StopCoroutine(panCo);
        var start = (manualAnchor != null ? manualAnchor.position : playerVCam.transform.position);
        var target = new Vector3(start.x, y, start.z); // 保持 z 不变（正交相机）

        panCo = StartCoroutine(
            LerpVector3(
                () => manualAnchor.position,
                v => manualAnchor.position = v,
                target,
                duration
            )
        );
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
        float current = playerVCam.m_Lens.OrthographicSize;
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
            var lens = playerVCam.m_Lens;
            lens.OrthographicSize = targetSize;
            playerVCam.m_Lens = lens;
            return;
        }

        zoomCo = StartCoroutine(
            LerpFloat(
                () => playerVCam.m_Lens.OrthographicSize,
                v =>
                {
                    var lens = playerVCam.m_Lens;
                    lens.OrthographicSize = v;
                    playerVCam.m_Lens = lens;
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
}
