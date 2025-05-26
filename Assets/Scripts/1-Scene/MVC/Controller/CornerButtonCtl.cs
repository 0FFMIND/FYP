using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using AudioSystem;
using TMPro;
using System;

namespace MVC
{
    [RequireComponent(typeof(Button))]
    public class CornerButtonCtl : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("四角标记")]
        public GameObject cornerTL;
        public GameObject cornerTR;
        public GameObject cornerBL;
        public GameObject cornerBR;
        [Header("背景 & 文本 切换")]
        public Image targetImage;          // 挂想换贴图的 Image
        public Sprite normalSprite;        // 未悬浮时的贴图1
        public Sprite hoverSprite;         // 悬浮时的贴图2
        [Header("点击/选择后的样式切换")]
        public GameObject tickMark;      // 子物体 Tick
        public TextMeshProUGUI targetText; // 挂在 Image 下的 TextMeshProUGUI
        public Color normalTextColor = Color.black;
        public Color hoverTextColor = Color.white;
        //
        private Action<int> _onChoice;    // 存外部传入的回调
        private int _choiceId;            // 存本按钮对应的 targetNodeId
        private RectTransform rect;
        private Button btn;
        private ColorBlock defaultColors;
        private Sprite _originalSprite;
        [Header("点击动画设置")]
        public float clickAnimDuration;     // 总时长
        public float shrinkFactor;         // 最小缩放
        public float expandFactor;         // 最大缩放

        private void Awake()
        {
            // 记录原始，用于恢复
            if (targetImage != null)
                _originalSprite = targetImage.sprite;
            btn = GetComponent<Button>();
            SetCorners(false);
        }
        public void SetupChoice(int choiceId, Action<int> onChoice)
        {
            _choiceId = choiceId;
            _onChoice = onChoice;
        }

        // 鼠标移入：显示四角
        public void OnPointerEnter(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySFX("hover");
            SetCorners(true);
            // 切换背景 & 文字
            if (targetImage != null && hoverSprite != null)
                targetImage.sprite = hoverSprite;
            if (targetText != null)
                targetText.color = hoverTextColor;
        }

        // 鼠标移出：隐藏四角
        public void OnPointerExit(PointerEventData eventData)
        {
            SetCorners(false);
            // 恢复背景 & 文字
            if (targetImage != null)
                targetImage.sprite = normalSprite ? normalSprite : _originalSprite;
            if (targetText != null)
                targetText.color = normalTextColor;
        }

        // 点击时：闪烁
        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySFX("click");
            // SetCorners(false);
            // 恢复背景 & 文字
            if (targetImage != null)
                targetImage.sprite = normalSprite ? normalSprite : _originalSprite;
            if (targetText != null)
                targetText.color = normalTextColor;
            // 启动点击动画
            btn.interactable = false;
            StartCoroutine(ClickFeedbackCoroutine());
        }

        private IEnumerator ClickFeedbackCoroutine()
        {
            rect = GetComponent<RectTransform>();
            Vector3 original = rect.localScale;
            float half = clickAnimDuration / 2;

            // 第一半：缩到 shrinkFactor
            for (float t = 0; t < half; t += Time.unscaledDeltaTime)
            {
                float p = t / half;
                rect.localScale = Vector3.Lerp(original, original * shrinkFactor, p);
                yield return null;
            }
            rect.localScale = original * shrinkFactor;

            // 第二半：放到 expandFactor
            for (float t = 0; t < half; t += Time.unscaledDeltaTime)
            {
                float p = t / half;
                rect.localScale = Vector3.Lerp(original * shrinkFactor, original * expandFactor, p);
                yield return null;
            }
            rect.localScale = original * expandFactor;

            // 抖一下回到原始大小
            rect.localScale = original;

            // 动画结束后，触发存下来的回调
            _onChoice?.Invoke(_choiceId);

            SetCorners(false);
            btn.interactable = true;
        }

        public void SetVisited(bool visited)
        {
            // 如果 Button 在同一个 GameObject 上：
            btn = GetComponent<Button>();
            // 如果 Button 在这个物体下面的某个子节点上，用这个：
            if (btn == null)
                btn = GetComponentInChildren<Button>();
            // 先改 Button.colors 里的 normalColor
            var cb = btn.colors;
            if (visited)
            {
                cb.normalColor = Color.gray;
                cb.highlightedColor = Color.gray; // 高亮也保持同色，按需调整
                cb.pressedColor = Color.black;
                cb.selectedColor = Color.gray;
                btn.colors = cb;
            }
            // 再显示或隐藏勾
            tickMark.SetActive(visited);
        }

        private void SetCorners(bool on)
        {
            cornerTL.SetActive(on);
            cornerTR.SetActive(on);
            cornerBL.SetActive(on);
            cornerBR.SetActive(on);
        }

    }

}
