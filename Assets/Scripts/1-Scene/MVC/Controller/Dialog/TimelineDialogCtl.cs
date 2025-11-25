using System;
using System.Collections;
using System.Collections.Generic;
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
        private bool enlargeArrow = false;

        [SerializeField]
        private bool skipBG = false;

        [SerializeField]
        private bool delayFinish = false;

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

        // 专门给interact的方法
        public void StartInteractDialogue(LineMapping[] mappings, string[] lines, Action onFinished)
        {
            this.mappings = mappings;
            finished = onFinished;
            StartDialogue(new DialogueModel(lines));
        }

        public void StartDialogue(int index, Action onFinished)
        {
            if (dialogueClips == null)
            {
                Debug.LogError("[TimelineDialogCtl]: Dialogue Clips is not assigned");
                return;
            }
            index = index - 1; // 用户传进来的从1开始
            mappings = dialogueClips.dialogMappings[index].mappings;
            finished = onFinished;
            string modelText = dialogueClips.dialogMappings[index].textFile;
            base.StartDialogue(new DialogueModel(modelText));
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

        private IEnumerator PlayClosed()
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
            // 隐藏对话与背景
            HideDialogue();
            dialogPanel.gameObject.SetActive(false);
        }

        private IEnumerator PlayClosedWithFinished()
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
            // 隐藏对话与背景
            HideDialogue();
            dialogPanel.gameObject.SetActive(false);
            // 再回调
            finished?.Invoke();
        }

        private void Close()
        {
            StartCoroutine(PlayClosed());
        }

        private void CloseAndFinish()
        {
            if (delayFinish)
            {
                // 触发对话结束回调
                var cb = finished;
                finished = null;
                cb?.Invoke();
            }
            else
            {
                StartCoroutine(PlayClosedWithFinished());
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
                            Close();
                        },
                        choiceModel
                    );
                }
                else
                {
                    CloseAndFinish();
                }
                return; // 这里务必 return，防止继续往下跑
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
