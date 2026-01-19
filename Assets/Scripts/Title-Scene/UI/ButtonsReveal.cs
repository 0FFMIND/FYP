using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsReveal : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private RectTransform[] buttons;     // 按顺序填

    [Header("Animation")]
    [SerializeField] private float offsetY = 40f;         // 从下方偏移多少开始“上浮”
    [SerializeField] private float eachTime = 0.35f;      // 每个按钮显示/上浮时长
    [SerializeField] private float interval = 0.12f;      // 下一个按钮开始的间隔
    [SerializeField] private bool fadeIn = true;          // 是否淡入
    [SerializeField] private bool moveUp = true;          // 是否上浮
    // 存储每个按钮的信息的结构体
    private struct Item
    {
        public RectTransform rect;
        public CanvasGroup cg;
        public Vector2 targetPos;
        public Vector2 startPos;
    }

    private List<Item> items;

    private void Awake()
    {
        RectTransform[] list = buttons;
        // 初始化items列表
        items = new List<Item>(list.Length);
        for (int i = 0; i < list.Length; i++)
        {
            var rt = list[i];
            if (rt == null) continue;
            var cg = rt.GetComponent<CanvasGroup>();
            if (cg == null) cg = rt.gameObject.AddComponent<CanvasGroup>();
            var target = rt.anchoredPosition;
            var start = target + (moveUp ? new Vector2(0f, -offsetY) : Vector2.zero);
            // 初始隐藏且不可点
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            if (moveUp) rt.anchoredPosition = start;

            items.Add(new Item { rect = rt, cg = cg, targetPos = target, startPos = start });
        }
    }

    public void PlayButton()
    {
        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        // 按顺序播放每个按钮的动画
        for (int i = 0; i < items.Count; i++)
        {
            StartCoroutine(AnimateOne(items[i]));
            yield return new WaitForSecondsRealtime(interval);
        }
    }

    private IEnumerator AnimateOne(Item it)
    {
        float t = 0f;
        float dur = Mathf.Max(0.0001f, eachTime);

        // 开始前确保隐藏不可点
        it.cg.alpha = 0f;
        it.cg.interactable = false;
        it.cg.blocksRaycasts = false;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / dur);
            // SmoothStep 缓入缓出
            float eased = p * p * (3f - 2f * p);

            if (moveUp)
                it.rect.anchoredPosition = Vector2.LerpUnclamped(it.startPos, it.targetPos, eased);

            if (fadeIn)
                it.cg.alpha = eased;

            yield return null;
        }

        if (moveUp) it.rect.anchoredPosition = it.targetPos;
        it.cg.alpha = 1f;
        it.cg.interactable = true;
        it.cg.blocksRaycasts = true;
    }
}
