using UnityEngine;

public class TitleParallaxUI : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        public RectTransform target;
        [Tooltip("该层最大偏移（像素）。越大动得越明显。")]
        public float maxOffset = 10f;
    }

    [Header("Layers")]
    public Layer frontTree;
    public Layer backTree;

    [Header("Motion")]
    [Tooltip("跟随速度，越大越灵敏。")]
    public float followSpeed = 8f;

    [Tooltip("整体最大归一化输入强度，防止鼠标到边缘时过大。一般 1 就够。")]
    public float inputStrength = 1f;

    private Vector2 buttonStart, titleStart, frontStart, backStart;

    private void Awake()
    {
        if (frontTree.target) frontStart = frontTree.target.anchoredPosition;
        if (backTree.target) backStart = backTree.target.anchoredPosition;
    }

    private void Update()
    {
        // 鼠标位置归一化到 [-0.5, 0.5]，再映射到 [-1, 1]
        Vector2 m = Input.mousePosition;
        Vector2 n = new Vector2(
            (m.x / Screen.width - 0.5f) * 2f,
            (m.y / Screen.height - 0.5f) * 2f
        );

        n = Vector2.ClampMagnitude(n * inputStrength, 1f);

        ApplyLayer(frontTree, frontStart, n);
        ApplyLayer(backTree, backStart, n);
    }

    private void ApplyLayer(Layer layer, Vector2 start, Vector2 n)
    {
        if (layer.target == null)
        {
            Debug.LogWarning($"[TitleParallaxUI]： '{layer.ToString()}' 的 target 为空，请在 Inspector 中为该层拖拽 RectTransform");
            return;
        }

        // 鼠标往右，上层往右上移动；想反向就乘 -1
        Vector2 targetPos = start + n * layer.maxOffset;

        layer.target.anchoredPosition = Vector2.Lerp(
            layer.target.anchoredPosition,
            targetPos,
            1f - Mathf.Exp(-followSpeed * Time.unscaledDeltaTime)
        );
    }
}
