using System;
using System.Collections;
using Manager;
using UnityEngine;

namespace MVC
{
    public class TimelineDialogCtl : DialogCtlBase
    {
        [SerializeField]
        EnterAnim enterAnim;
        // Unity不能直接序列化接口，所以用序列化的MonoBehaviour来间接持有接口引用
        [SerializeField] private MonoBehaviour clipProviderBehaviour;
        private IDialogueClipProvider clipProvider;

        [SerializeField]
        private GameObject dialogPanel;

        private Action finished;

        private bool _waitingChoice = false;

        private LineMapping[] mappings;

        public ChoiceModel choiceModel;

        protected Sprite currentSprite;

        [SerializeField]
        private ChoiceCtl choiceCtl;

        [SerializeField]
        private bool skipBG = false;

        [SerializeField]
        private bool earlyFinish = false;

        private void Awake()
        {
            // 获取接口引用
            clipProvider = clipProviderBehaviour as IDialogueClipProvider;
            // 隐藏dialog
            HideDialogue();
        }

        private void HideDialogue()
        {
            // 隐藏内容
            RenderViews(null, null);
            dialogPanel.gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// 由 InteractCtl 调用：传入行映射 + 文本行数组。
        /// </summary>
        public void StartInteractDialogue(LineMapping[] mappings, string[] lines, Action onFinished)
        {
            this.mappings = mappings;
            finished = onFinished;
            base.StartDialogue(new DialogueModel(lines));
        }

        // Scene1 的便利重载：外部可以直接传 enum
        public void StartClipDialogue(Scene1DialogueId id, Action onFinished)
        {
            StartClipDialogue((int)id, onFinished);
        }

        /// <summary>
        /// 由 Timeline 调用：通过枚举 ID 从 ScriptableObject 里取文本。
        /// </summary>
        private void StartClipDialogue(int clipId, Action onFinished)
        {
            if (clipProvider == null)
            {
                Debug.LogError("[TimelineDialogCtl]: 未绑定 IDialogueClipProvider");
                return;
            }

            DialogueClipBase clip = clipProvider.GetClip(clipId);
            if (clip == null)
            {
                Debug.LogError($"[TimelineDialogCtl]: 找不到对应对话配置 id = {clipId}");
                return;
            }

            mappings = clip.mappings;
            finished = onFinished;

            base.StartDialogue(new DialogueModel(clip.textFile));
        }

        protected override IEnumerator TypeLines(Sprite currentSprite = null)
        {
            // 如果是第一句
            if (index == 0)
            {
                _isEntering = true;
                RenderViews(currentSprite, null);
                dialogPanel.gameObject.SetActive(true);

                // 准备需要播放的动画协程；BG 可以按 skipBG 决定是否为 null
                var dialogAnim = enterAnim.PlayScriptedEnterAnim(dialogueRenderer.dialogueView, false);
                IEnumerator bgAnim = null;
                if (!skipBG)
                {
                    bgAnim = enterAnim.PlayScriptedEnterAnim(dialogueRenderer.bgView, true);
                }

                // 在 EnterAnim 里统一并行播放并等待全部完成
                yield return enterAnim.RunAllAndWait(dialogAnim, bgAnim);

                _isEntering = false;
                // 如果跳过背景动画（通常是全屏贴图），做个淡入
                if (skipBG)
                {
                    TransitionMgr.Instance.FadeIn(0.5f);
                }
            }
            yield return base.TypeLines(currentSprite);
        }

        private IEnumerator EndDialogueWithExitAnimation()
        {
            dialogueRenderer.Hide();

            // 对话框和 BG 的退场动画都交给 EnterAnim，并等待两者完成
            var dialogAnim = enterAnim != null
                ? enterAnim.PlayScriptedExitAnim(dialogueRenderer.dialogueView, false)
                : null;
            var bgAnim = enterAnim != null
                ? enterAnim.PlayScriptedExitAnim(dialogueRenderer.bgView, true)
                : null;

            yield return enterAnim.RunAllAndWait(dialogAnim, bgAnim);

            // 再回调
            finished?.Invoke();
        }

        private void EndDialogue()
        {
            if (earlyFinish)
            {
                // 不播放面板退出动画，直接回调
                // 用于UI场景中，避免直接关闭Canvas再进行场景切换的淡出动画
                var cb = finished;
                finished = null;
                cb?.Invoke();
            }
            else
            {
                StartCoroutine(EndDialogueWithExitAnimation());
            }
        }

        protected override void NextLine()
        {
            // 若正在等选项，直接忽略一切推进输入
            if (_waitingChoice)
            {
                return;
            }
            // 读完
            if (index == dialogueModel.Lines.Length)
            {
                if (choiceModel.items != null
                    && choiceModel.items.Length > 0
                    && choiceCtl != null
                )
                {
                    _waitingChoice = true; // 上锁，防止重复 Show
                    choiceCtl.ShowWithClosed(
                        () =>
                        {
                            _waitingChoice = false;
                            EndDialogue();
                        },
                        choiceModel
                    );
                }
                else
                {
                    EndDialogue();
                }
                // 防止继续往下跑
                return;
            }
            // 不然按钮点击会误认为nextline
            if (dialogueModel == null || index >= dialogueModel.Lines.Length)
            {
                return;
            }
            foreach (var map in mappings)
            {
                if (index == map.lineIndex)
                {
                    currentSprite = map.sprite;
                    // 触发所有绑定的行为
                    map.onEnter?.Invoke();
                    break;
                }
            }
            // 打字
            StartCoroutine(TypeLines(currentSprite));
        }
    }
}
