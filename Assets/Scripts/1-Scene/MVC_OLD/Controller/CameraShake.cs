using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // 震动幅度和持续时间
    public float duration;
    public float magnitude;

    private Vector3 _originalPos;

    private void Awake()
    {
        // 记录摄像机初始位置
        _originalPos = transform.localPosition;
    }

    /// <summary>
    /// 外部调用：开始一次震动
    /// </summary>
    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(DoShake());
    }

    private IEnumerator DoShake()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 生成一个随机偏移量
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = _originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 恢复原位
        transform.localPosition = _originalPos;
    }
}
