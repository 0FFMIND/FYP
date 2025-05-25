using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // �𶯷��Ⱥͳ���ʱ��
    public float duration;
    public float magnitude;

    private Vector3 _originalPos;

    private void Awake()
    {
        // ��¼�������ʼλ��
        _originalPos = transform.localPosition;
    }

    /// <summary>
    /// �ⲿ���ã���ʼһ����
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
            // ����һ�����ƫ����
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = _originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // �ָ�ԭλ
        transform.localPosition = _originalPos;
    }
}
