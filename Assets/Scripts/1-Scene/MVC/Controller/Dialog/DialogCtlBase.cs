using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace MVC
{
    [System.Serializable]
    public struct LineMapping
    {
        [Tooltip("当 index 等于此值时，切换到对应的 sprite")]
        public int lineIndex;

        [Tooltip("切换时使用的 Sprite")]
        public Sprite sprite;

        [Tooltip("触发行为（可在 Inspector 里添加多个回调）")]
        public UnityEvent onEnter;
    }

    public abstract class DialogCtlBase : MonoBehaviour
    {
        protected bool _isEntering;

        [Header("ScriptableObject 对话资源")]
        [SerializeField]
        protected string modelText;

        [SerializeField]
        protected LineMapping[] mappings;

        [Header("Typing")]
        [SerializeField]
        private bool enableTypingSfx = true;

        [SerializeField, Min(0.01f)]
        private float typingRate = 1f;

        [Header("视图引用")]
        [SerializeField]
        protected DialogueView bgView;

        [SerializeField]
        protected DialogueView dialogueView;

        protected int index;
        protected DialogueModel dialogueModel;
        protected Sprite currentSprite;
        protected Coroutine typingCoroutine;
        protected ArrowIndicator arrowIndicator;

        private float typeSpeed;
        private LanguageCode languageCode;

        // 打字进度
        private int typingTotal;
        private float revealProgress;

        protected virtual void Awake()
        {
            // 获取箭头指示器
            arrowIndicator = GetComponent<ArrowIndicator>();
            if (arrowIndicator == null)
                // 若不存在则添加
                arrowIndicator = gameObject.AddComponent<ArrowIndicator>();
        }

        public virtual void StartDialogue()
        {
            arrowIndicator.EnsureCreated(this.transform);

            if (modelText != null && modelText.Length > 0)
            {
                // 载入对话
                dialogueModel = new DialogueModel(modelText);
            }
            // 启动时记录当前语言
            languageCode = SettingsMgr.Instance.GetLanguage();
            // 刷新index
            index = 0;
            NextLine();
        }

        // 基类提供统一渲染方法，子类可直接调用
        protected void RenderViews(Sprite sprite, string text)
        {
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

        private void RevealAllNow()
        {
            var tmp = dialogueView.tmp;
            if (!tmp)
                return;
            revealProgress = 1f;
            // 直接拉满可见字符，避免依赖 textInfo 的计数时机
            tmp.maxVisibleCharacters = int.MaxValue;
            // 强制刷新
            tmp.ForceMeshUpdate();
            // 显示箭头
            arrowIndicator.PositionArrowUnderText(dialogueView.tmp);
        }

        protected virtual IEnumerator TypeLines()
        {
            string fullRaw = "";
            // 在真正渲染前，用“当前行”的最新文本覆盖 fullRaw
            if (
                dialogueModel != null
                && dialogueModel.Lines != null
                && dialogueModel.Lines.Length > 0
            )
            {
                // NextLine() 渲染后才 index++，故此处屏上应是 index-1
                int cur = Mathf.Clamp(index, 0, dialogueModel.Lines.Length - 1);
                // 用最新（可能已 Reload 后）的文本覆盖
                fullRaw = dialogueModel.Lines[cur] ?? fullRaw;
            }
            // 一次性设置完整文本，然后用 maxVisibleCharacters 揭示
            RenderViews(currentSprite, fullRaw);
            var tmp = dialogueView.tmp;
            tmp.ForceMeshUpdate();
            tmp.maxVisibleCharacters = 0;

            typingTotal = tmp.textInfo.characterCount;
            int cnt = 0;
            revealProgress = 0f;
            for (; ; )
            {
                // 若外部（语言切换）导致文本变化，这里会看到最新的 typingTotal
                int curTotal = typingTotal;
                int visNow = tmp.maxVisibleCharacters;
                if (visNow >= curTotal)
                    break;

                tmp.maxVisibleCharacters = visNow + 1;
                revealProgress = Mathf.Clamp01(
                    (float)tmp.maxVisibleCharacters / Mathf.Max(1, typingTotal)
                );
                // 英文两字符一个音效；中文每个字符一个音效
                bool isEn = SettingsMgr.Instance.GetLanguage() == LanguageCode.en;
                if (isEn)
                {
                    cnt++;
                    if (cnt >= 2)
                    {
                        cnt = 0;
                        if (enableTypingSfx)
                        {
                            AudioManager.Instance.PlaySFX("typing");
                        }
                    }
                }
                else
                {
                    if (enableTypingSfx)
                    {
                        AudioManager.Instance.PlaySFX("typing");
                    }
                }
                float wait = typeSpeed / typingRate;
                if (isEn)
                    wait *= 0.5f;
                yield return new WaitForSeconds(wait);
            }

            // 完成后显示箭头
            arrowIndicator.PositionArrowUnderText(dialogueView.tmp);
            typingCoroutine = null;
            revealProgress = 1f;
        }

        private void OnDialogueClick()
        {
            // 如果正在加载panel
            if (_isEntering || dialogueModel == null)
            {
                return;
            }
            if (typingCoroutine != null)
            {
                // 先暂停
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                if (index <= dialogueModel.Lines.Length)
                {
                    RevealAllNow();
                    revealProgress = 1f;
                    // 开启小箭头
                    arrowIndicator.PositionArrowUnderText(dialogueView.tmp);
                }
                else
                {
                    // 结束的时候清空
                    RenderViews(null, null);
                    // 关掉小箭头
                    arrowIndicator.Hide();
                    arrowIndicator.SetColor(Color.white);
                }
            }
            else
            {
                // 移动到下一个line
                index++;
                NextLine();
            }
        }

        // 由子类实现：推进到下一行
        protected abstract void NextLine();



        private void OnLanguageChanged(ELanguageChanged e)
        {
            // 仅当语言变化时，手动触发重译并刷新当前行
            if (e.Language != languageCode)
            {
                languageCode = e.Language;
                if (dialogueModel != null && dialogueView != null)
                {
                    // 让 Model 重新生成 Lines
                    dialogueModel.Reload();
                    // 直接改 TMP 的 text，并保留当前遮罩
                    var tmp = dialogueView.tmp;
                    if (tmp && tmp.text != null && tmp.text.Length > 0)
                    {
                        // 当前显示的是 index
                        int cur = Mathf.Clamp(index, 0, dialogueModel.Lines.Length - 1);
                        // 保留“旧进度百分比”。如果之前没统计，按当前可见/旧总长兜底
                        float prevProgress =
                            revealProgress > 0f
                                ? Mathf.Clamp01(revealProgress)
                                : Mathf.Clamp01(
                                    (float)tmp.maxVisibleCharacters / Mathf.Max(1, typingTotal)
                                );
                        // 换新文本
                        if (dialogueModel.Lines.Length > 0)
                        {
                            tmp.text = dialogueModel.Lines[cur];
                            tmp.ForceMeshUpdate();
                            // 按百分比映射到新长度
                            int newLen = tmp.textInfo.characterCount;
                            typingTotal = newLen;
                            tmp.maxVisibleCharacters = Mathf.RoundToInt(prevProgress * newLen);
                            // 同步内部进度
                            revealProgress = prevProgress;
                            // 重新定位箭头
                            if (arrowIndicator.gameObject.activeSelf)
                            {
                                arrowIndicator.PositionArrowUnderText(tmp);
                            }
                        }
                    }
                }
            }
        }

        // 订阅输入与设置变更事件
        protected virtual void OnEnable()
        {
            EventBus.Subscribe<EInputPressed, InputAction>(
                InputAction.DialogueClick,
                OnDialogueClick
            );
            EventBus.Subscribe<ESettingsChanged>(OnSettingsChanged);
            EventBus.Subscribe<ELanguageChanged>(OnLanguageChanged);
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
            EventBus.Unsubscribe<ELanguageChanged>(OnLanguageChanged);

            EventBus.Unsubscribe<EInputPressed, InputAction>(
                InputAction.DialogueClick,
                OnDialogueClick
            );
        }
    }
}
