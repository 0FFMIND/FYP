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

        [Header("打字机设置")]
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

        private LanguageCode languageCode;

        [SerializeField]
        protected DialogueRenderer dialogueRenderer;

        public void RenderViews(Sprite sprite, string text)
        {
            dialogueRenderer.RenderViews(sprite, text);
        }
        public virtual void StartDialogue()
        {
            // 载入对话
            if (modelText != null && modelText.Length > 0)
            {
                dialogueModel = new DialogueModel(modelText);
            }
            // 启动时记录当前语言
            languageCode = SettingsMgr.Instance.GetLanguage();
            // 刷新index
            index = 0;
            // 开始打字
            NextLine();
        }

        public void HideArrow()
        {
            dialogueRenderer.Hide();
        }

        protected virtual IEnumerator TypeLines()
        {
            string fullRaw = "";

            if (
                dialogueModel != null
                && dialogueModel.Lines != null
                && dialogueModel.Lines.Length > 0
            )
            {
                int cur = Mathf.Clamp(index, 0, dialogueModel.Lines.Length - 1);
                // 取出当前行的文本
                fullRaw = dialogueModel.Lines[cur] ?? fullRaw;
            }
            // 交给 dialogueRenderer 进行显示和打字
            dialogueRenderer.ShowLine(currentSprite, fullRaw);

            yield return null;
        }

        private void OnDialogueClick()
        {
            // 如果正在加载panel
            if (_isEntering || dialogueModel == null)
            {
                return;
            }
            // 如果正在打字，跳过打字并显示
            if (dialogueRenderer.IsTyping)
            {
                dialogueRenderer.SkipTyping();
                return;
            }
            // 移动到下一个line
            index++;
            NextLine();
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
                    int cur = Mathf.Clamp(index, 0, dialogueModel.Lines.Length - 1);
                    string newText = dialogueModel.Lines[cur] ?? "";
                    // 让 Renderer 重新绑定语言
                    dialogueRenderer.RebindLanguage(currentSprite, newText);
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
            EventBus.Subscribe<ELanguageChanged>(OnLanguageChanged);
        }

        // 取消订阅事件
        protected virtual void OnDisable()
        {
            EventBus.Unsubscribe<ELanguageChanged>(OnLanguageChanged);

            EventBus.Unsubscribe<EInputPressed, InputAction>(
                InputAction.DialogueClick,
                OnDialogueClick
            );
        }
    }
}
