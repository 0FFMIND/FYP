using System.Collections;
using UnityEngine;

public class ObjBounce : MonoBehaviour
{
    [Tooltip("向下位移的距离（世界坐标单位）")]
    public float downDistance = 0.07f;

    [Tooltip("下行耗时（秒）")]
    public float downTime = 0.4f;

    [Tooltip("上行耗时（秒）")]
    public float upTime = 0.4f;

    [Tooltip("启用时自动播放")]
    public bool playOnEnable = true;

    private Vector3 _pivot;       // 以当前点为枢轴
    private Coroutine _co;        // 跑动协程

    private void OnEnable()
    {
        SetPivotHere();           // 物体在哪里就以哪里为 pivot
        if (playOnEnable) Play();
    }

    private void OnDisable()
    {
        Stop();
        // 停止后回到枢轴，避免残留偏移
        transform.position = _pivot;
    }

    /// 以当前 Transform 位置作为新的枢轴
    public void SetPivotHere()
    {
        _pivot = transform.position;
    }

    public void Play()
    {
        if (_co != null) return;
        _co = StartCoroutine(CoBounce());
    }

    public void Stop()
    {
        if (_co == null) return;
        StopCoroutine(_co);
        _co = null;
    }

    private IEnumerator CoBounce()
    {
        for (; ; )
        {
            // 中 -> 下
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / Mathf.Max(0.0001f, downTime);
                float d = Mathf.Lerp(0f, downDistance, t);
                transform.position = _pivot + Vector3.down * d;
                yield return null;
            }

            // 下 -> 中
            t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / Mathf.Max(0.0001f, upTime);
                float d = Mathf.Lerp(downDistance, 0f, t);
                transform.position = _pivot + Vector3.down * d;
                yield return null;
            }

            // 中 -> 上
            t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / Mathf.Max(0.0001f, downTime);
                float d = Mathf.Lerp(0f, downDistance, t);
                transform.position = _pivot + Vector3.up * d;
                yield return null;
            }

            // 上 -> 中
            t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / Mathf.Max(0.0001f, upTime);
                float d = Mathf.Lerp(downDistance, 0f, t);
                transform.position = _pivot + Vector3.up * d;
                yield return null;
            }
        }
    }

}
