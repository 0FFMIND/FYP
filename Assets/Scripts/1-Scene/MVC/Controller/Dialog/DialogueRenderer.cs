
using System;
using System.Collections;
using UnityEngine;
using Utils;
namespace MVC
{
    public class DialogueRenderer : MonoBehaviour
    {
        [Header("视图引用")]
        [SerializeField]
        protected DialogueView bgView;

        [SerializeField]
        protected DialogueView dialogueView;

        [Header("打字机设置")]
        [SerializeField]
        private bool enableTypingSfx = true;

        [SerializeField, Min(0.01f)]
        private float typingRate = 1f;

        [SerializeField, Tooltip("是否放大箭头指示器")]
        private bool enlargeArrow = false;
        public bool IsTyping => typewriter != null && typewriter.IsTyping;
        private float typeSpeed;
        public ArrowIndicator arrowIndicator;
        private Typewriter typewriter;
        private Coroutine waitArrowCo;

        public void Hide()
        {
            arrowIndicator.Hide();
        }

        // 负责确保本对象上存在对话所需的辅助组件（ArrowIndicator 与 Typewriter）
        private void Awake()
        {
            // 获取箭头指示器
            arrowIndicator = GetComponent<ArrowIndicator>();
            if (arrowIndicator == null)
            {
                arrowIndicator = gameObject.AddComponent<ArrowIndicator>();
            }

            // 获取打字机组件
            typewriter = GetComponent<Typewriter>();
            if (typewriter == null)
            {
                typewriter = gameObject.AddComponent<Typewriter>();
            }

            // 确保箭头实例已创建
            arrowIndicator.EnsureCreated(transform);
            if (enlargeArrow)
            {
                enlargeArrow = false;
                arrowIndicator.SetArrowScale(100f);
            }
        }

        public void RenderViews(Sprite sprite, string text)
        {
            if (dialogueView != null && !dialogueView.gameObject.activeInHierarchy) return;
            if (bgView != null && !bgView.gameObject.activeInHierarchy) return;
            if (bgView)
            {
                bgView.Render(sprite, null);
            }
            if (dialogueView)
            {
                if (text == null)
                {
                    dialogueView.gameObject.SetActive(false);
                }
                else
                {
                    if (!dialogueView.gameObject.activeSelf)
                    {
                        dialogueView.gameObject.SetActive(true);
                    }
                    dialogueView.Render(null, text);
                }
            }
        }

        public void SkipTyping()
        {
            // 停止逐字，显示全文
            typewriter.Skip();
            // 显示箭头
            arrowIndicator.PositionArrowUnderText(dialogueView.tmp);
        }

        public void RebindLanguage(Sprite currentSprite, string newText)
        {
            // 重新渲染视图
            RenderViews(currentSprite, newText);
            // 交给 Typewriter 处理映射
            typewriter.RebindAfterLanguageChange(newText);
            // 重新定位箭头
            if (IsTyping)
            {
                arrowIndicator.Hide();
                if (waitArrowCo != null) StopCoroutine(waitArrowCo);
                waitArrowCo = StartCoroutine(WaitAndShowArrow());
            }
            else
            {
                arrowIndicator.PositionArrowUnderText(dialogueView.tmp);
            }
        }

        public void ShowLine(Sprite currentSprite, string fullRaw)
        {
            if (dialogueView == null)
            {
                Debug.LogError("[DialogueRenderer] dialogueView 未绑定");
                return;
            }
            // 隐藏箭头提示
            arrowIndicator.Hide();
            // 一次性渲染完整文本，等待用 maxVisibleCharacters 揭示
            RenderViews(currentSprite, fullRaw);

            // 交给 Typewriter 处理逐字、音效和箭头
            typewriter.StartTyping(
                dialogueView.tmp,
                fullRaw,
                typeSpeed,
                typingRate,
                enableTypingSfx,
                arrowIndicator
            );

            // 等打字结束再显示箭头
            if (waitArrowCo != null) StopCoroutine(waitArrowCo);
            waitArrowCo = StartCoroutine(WaitAndShowArrow());
        }

        private IEnumerator WaitAndShowArrow()
        {
            yield return new WaitUntil(() => !IsTyping);
            arrowIndicator.PositionArrowUnderText(dialogueView.tmp);
            waitArrowCo = null;
        }
        // 订阅设置变更事件
        private void OnEnable()
        {
            EventBus.Subscribe<ESettingsChanged>(OnSettingsChanged);
            arrowIndicator.Hide();
        }

        // 设置打字速度
        private void OnSettingsChanged(ESettingsChanged e)
        {
            typeSpeed = e.Settings.typeSpeed;
        }

        // 取消订阅事件
        protected virtual void OnDisable()
        {
            EventBus.Unsubscribe<ESettingsChanged>(OnSettingsChanged);
            arrowIndicator.Hide();
        }
    }
}