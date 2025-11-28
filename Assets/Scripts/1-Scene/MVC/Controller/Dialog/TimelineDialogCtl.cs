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

        [SerializeField]
        private Scene1DialogueClips dialogueClips;

        [SerializeField]
        private GameObject dialogPanel;
        private Action finished;

        // 是否在本轮结束时进入 Choice 流程（不立即关闭面板）
        public bool hasChoice = false;

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

        private void Start()
        {
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
            StartDialogue(new DialogueModel(lines));
        }

        /// <summary>
        /// 由 Timeline 调用：通过枚举 ID 从 ScriptableObject 里取文本。
        /// </summary>
        public void StartDialogue(Scene1DialogueId id, Action onFinished)
        {
            if (dialogueClips == null)
            {
                Debug.LogError("[TimelineDialogCtl]: 未绑定 Scene1DialogueClips 组件，请检查场景中的引用");
                return;
            }
            // 通过枚举 ID 从 clips 里找到对应配置
            var clip = dialogueClips.GetClip(id);
            if (clip == null)
            {
                Debug.LogError($"[TimelineDialogCtl]: 找不到对应的对话配置，dialogueId = {id}。请检查 Scene1DialogueClips 的 dialogMappings 配置");
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
                bool donePanel = false,
                    doneBG = false;
                dialogPanel.gameObject.SetActive(true);

                // 先做文本框上浮
                IEnumerator RunPanel()
                {
                    yield return enterAnim.PlayEnterCode(dialogueRenderer.dialogueView, false);
                    donePanel = true;
                }

                StartCoroutine(RunPanel());
                if (!skipBG)
                {
                    IEnumerator RunBG()
                    {
                        yield return enterAnim.PlayEnterCode(dialogueRenderer.bgView, true);
                        doneBG = true;
                    }
                    StartCoroutine(RunBG());
                }
                else
                {
                    doneBG = true;
                }
                // 同时开跑
                yield return new WaitUntil(() => donePanel && doneBG);
                _isEntering = false;
                if (skipBG)
                {
                    TransitionMgr.Instance.FadeIn(0.5f);
                }
            }
            yield return base.TypeLines(currentSprite);
        }

        private IEnumerator PlayCloseAndFinished()
        {
            dialogueRenderer.Hide();
            // 并行把“对话框 & 背景”做 CodeTween 退场，等两者都结束
            bool donePanel = false,
                doneBG = false;

            IEnumerator RunPanel()
            {
                if (enterAnim)
                    yield return enterAnim.PlayExitCode(dialogueRenderer.dialogueView, false);
                donePanel = true;
            }
            IEnumerator RunBG()
            {
                if (enterAnim)
                    yield return enterAnim.PlayExitCode(dialogueRenderer.bgView, true);
                doneBG = true;
            }

            StartCoroutine(RunPanel());
            StartCoroutine(RunBG());
            yield return new WaitUntil(() => donePanel && doneBG);
            // 再回调
            finished?.Invoke();
        }

        private void CloseAndFinish()
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
                StartCoroutine(PlayCloseAndFinished());
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
                if (
                    hasChoice
                    && choiceModel.items != null
                    && choiceModel.items.Length > 0
                    && choiceCtl != null
                )
                {
                    _waitingChoice = true; // 上锁，防止重复 Show
                    choiceCtl.ShowWithClosed(
                        () =>
                        {
                            _waitingChoice = false;
                            CloseAndFinish();
                        },
                        choiceModel
                    );
                }
                else
                {
                    CloseAndFinish();
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
