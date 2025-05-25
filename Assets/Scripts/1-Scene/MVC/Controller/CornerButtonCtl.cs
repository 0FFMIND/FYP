using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using AudioSystem;
using TMPro;

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
        private Button btn;
        private ColorBlock defaultColors;
        private Sprite _originalSprite;
        // 闪烁用
        public float flashDuration = 0.2f;
        public int flashCount = 3;

        private void Awake()
        {
            // 记录原始，用于恢复
            if (targetImage != null)
                _originalSprite = targetImage.sprite;
            btn = GetComponent<Button>();
            SetCorners(false);
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
            SetCorners(false);
            // 恢复背景 & 文字
            if (targetImage != null)
                targetImage.sprite = normalSprite ? normalSprite : _originalSprite;
            if (targetText != null)
                targetText.color = normalTextColor;
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
