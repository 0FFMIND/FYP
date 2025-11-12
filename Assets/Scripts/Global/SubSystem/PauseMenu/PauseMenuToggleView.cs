using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Manager;

namespace MVC
{
    public class PauseMenuToggleView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private Toggle toggle;

        [SerializeField]
        public TMP_Text text;

        [Header("Background")]
        [SerializeField]
        private Image background; // 行底色

        [SerializeField]
        private Color bgOn = new(1f, 1f, 1f, 1f); // 选中底色（常亮）

        [SerializeField]
        private Color bgOff = new(1f, 1f, 1f, 0.00f); // 未选中底色（透明）

        [Header("Text")]
        [SerializeField]
        private Color textOn = Color.black; // 选中文本：黑

        [SerializeField]
        private Color textOff = Color.white; // 未选中文本：白

        [Header("Hover")]
        [Range(0f, 1f)]
        [SerializeField]
        private float hoverAlpha = 0.3f; // 悬停白底透明度

        private bool _isHover;

        public event Action OnSelected;

        private bool _suppressNextSfx;

        // 绑定数据并指定所属 ToggleGroup
        public void Bind(ToggleGroup group)
        {
            // 加入互斥组
            toggle.group = group;
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(isOn =>
            {
                ApplyVisual(isOn);
                // 只有切为选中时广播选中事件
                if (isOn)
                {
                    if (!_suppressNextSfx)
                    {
                        AudioManager.Instance.PlaySFX("buttonClick");
                    }
                    OnSelected?.Invoke();
                }
            });
            ApplyVisual(toggle.isOn);
        }

        public void SetSelected(bool on, bool notify = false)
        {
            if (!toggle) return;
            _suppressNextSfx = true;
            if (notify) toggle.isOn = on;                // 触发回调 & 通知 ToggleGroup
            else toggle.SetIsOnWithoutNotify(on); // 仅改状态/视觉
            ApplyVisual(on);
            _suppressNextSfx = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHover = true;
            ApplyVisual(toggle && toggle.isOn);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHover = false;
            ApplyVisual(toggle && toggle.isOn);
        }

        private void ApplyVisual(bool isOn)
        {
            if (text)
                text.color = isOn ? textOn : textOff;

            if (background)
            {
                if (_isHover)
                {
                    // 悬停：固定白底 + 指定透明度
                    background.color = new Color(1f, 1f, 1f, hoverAlpha);
                }
                else
                {
                    // 非悬停：按选中/未选中底色
                    background.color = isOn ? bgOn : bgOff;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!toggle || !toggle.IsActive() || !toggle.IsInteractable()) return;
            // 用 Toggle 的 API 切换为选中（会触发 onValueChanged）
            toggle.isOn = true;
        }
    }
}
