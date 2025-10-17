using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerYSort : MonoBehaviour
{
    [Header("排序锚点（脚下）")]
    public Transform yAnchor; // 拖到“Feet”子物体；留空则用自身

    private SpriteRenderer sr;
    private float lastY;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        float y = (yAnchor ? yAnchor.position.y : transform.position.y);
        if (Mathf.Abs(y - lastY) > 0.0001f)
        {
            sr.sortingOrder = 2;
            lastY = y;
        }
    }
}
