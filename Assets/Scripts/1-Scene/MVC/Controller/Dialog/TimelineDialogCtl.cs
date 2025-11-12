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
        private LineMapping[] firstMappings;

        [SerializeField]
        private LineMapping[] secondMappings;

        [SerializeField]
        private LineMapping[] thirdMappings;

        [SerializeField]
        private LineMapping[] fourthMappings;

        [SerializeField]
        private LineMapping[] fifthMappings;

        [SerializeField]
        private LineMapping[] sixthMappings;

        [SerializeField]
        private LineMapping[] seventhMappings;

        [SerializeField]
        private LineMapping[] eighthMappings;

        [SerializeField]
        private LineMapping[] ninethMappings;

        [SerializeField]
        private LineMapping[] tenthMappings;

        [SerializeField]
        private LineMapping[] eleventhMappings;

        [SerializeField]
        private LineMapping[] twelfthMappings;

        [SerializeField]
        private LineMapping[] thirteenthMappings;

        [SerializeField]
        private GameObject dialogPanel;

        private Action finished;

        // 是否在本轮结束时进入 Choice 流程（不立即关闭面板）
        public bool hasChoice = false;

        private bool _waitingChoice = false;

        public ChoiceModel choiceModel;

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

        public override void StartDialogue()
        {
            base.StartDialogue();
        }

        // 专门给interact的方法
        public void StartInteractDialogue(LineMapping[] mappings, string[] lines, Action onFinished)
        {
            this.mappings = mappings;
            finished = onFinished;
            modelText = "";
            dialogueModel = new DialogueModel(lines);
            StartDialogue();
        }

        public void StartSecondDialogue(Action onFinished)
        {
            mappings = secondMappings;
            finished = onFinished;
            modelText = "1-Scene-2.txt";
            StartDialogue();
        }

        public void StartFirstDialogue(Action onFinished)
        {
            mappings = firstMappings;
            finished = onFinished;
            modelText = "1-Scene-1.txt";
            base.StartDialogue();
        }

        public void StartThirdDialogue(Action onFinished)
        {
            mappings = thirdMappings;
            finished = onFinished;
            modelText = "1-Scene-5.txt";
            base.StartDialogue();
        }

        public void StartFourthDialogue(Action onFinished)
        {
            mappings = fourthMappings;
            finished = onFinished;
            modelText = "1-Scene-7.txt";
            base.StartDialogue();
        }

        public void StartFifthDialogue(Action onFinished)
        {
            mappings = fifthMappings;
            finished = onFinished;
            modelText = "1-Scene-8.txt";
            base.StartDialogue();
        }

        public void StartSixthDialogue(Action onFinished)
        {
            mappings = sixthMappings;
            finished = onFinished;
            modelText = "1-Scene-9.txt";
            base.StartDialogue();
        }
        public void StartSeventhDialogue(Action onFinished)
        {
            mappings = seventhMappings;
            finished = onFinished;
            modelText = "1-Scene-10.txt";
            base.StartDialogue();
        }

        public void StartEighthDialogue(Action onFinished)
        {
            mappings = eighthMappings;
            finished = onFinished;
            modelText = "1-Scene-11.txt";
            base.StartDialogue();
        }

        public void StartNinethDialogue(Action onFinished)
        {
            mappings = ninethMappings;
            finished = onFinished;
            modelText = "1-Scene-12.txt";
            base.StartDialogue();
        }

        public void StartTenthDialogue(Action onFinished)
        {
            mappings = tenthMappings;
            finished = onFinished;
            modelText = "1-Scene-13.txt";
            base.StartDialogue();
        }

        public void StartEleventhDialogue(Action onFinished)
        {
            mappings = eleventhMappings;
            finished = onFinished;
            modelText = "1-Scene-14.txt";
            base.StartDialogue();
        }

        public void StartTwelfthDialogue(Action onFinished)
        {
            mappings = twelfthMappings;
            finished = onFinished;
            modelText = "1-Scene-15.txt";
            base.StartDialogue();
        }

        public void StartThirteenthDialogue(Action onFinished)
        {
            mappings = thirteenthMappings;
            finished = onFinished;
            modelText = "1-Scene-16.txt";
            base.StartDialogue();
        }
        protected override IEnumerator TypeLines()
        {
            arrow.gameObject.SetActive(false);
            if (enlargeArrow)
            {
                enlargeArrow = false;
                Vector3 scale = arrow.gameObject.transform.localScale;
                scale *= 100f;
                arrow.gameObject.transform.localScale = scale;
            }
            // 如果是第一句
            if (index == 0)
            {
                _isEntering = true;
                arrow.gameObject.SetActive(false);
                RenderViews(currentSprite, null);
                bool donePanel = false,
                    doneBG = false;
                dialogPanel.gameObject.SetActive(true);

                // 先做文本框上浮
                IEnumerator RunPanel()
                {
                    yield return enterAnim.PlayEnterCode(dialogueView, false);
                    donePanel = true;
                }

                StartCoroutine(RunPanel());
                if (!skipBG)
                {
                    IEnumerator RunBG()
                    {
                        yield return enterAnim.PlayEnterCode(bgView, true);
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
            yield return base.TypeLines();
        }

        private IEnumerator PlayClosed()
        {
            // 清空文本
            dialogueView.tmp.text = "";
            // 关掉向下小箭头
            arrow.gameObject.SetActive(false);
            // 并行把“对话框 & 背景”做 CodeTween 退场，等两者都结束
            bool donePanel = false,
                doneBG = false;

            IEnumerator RunPanel()
            {
                if (enterAnim)
                    yield return enterAnim.PlayExitCode(dialogueView, false);
                donePanel = true;
            }
            IEnumerator RunBG()
            {
                if (enterAnim)
                    yield return enterAnim.PlayExitCode(bgView, true);
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
            // 清空文本
            dialogueView.tmp.text = "";
            // 关掉向下小箭头
            arrow.gameObject.SetActive(false);
            // 并行把“对话框 & 背景”做 CodeTween 退场，等两者都结束
            bool donePanel = false,
                doneBG = false;

            IEnumerator RunPanel()
            {
                if (enterAnim)
                    yield return enterAnim.PlayExitCode(dialogueView, false);
                donePanel = true;
            }
            IEnumerator RunBG()
            {
                if (enterAnim)
                    yield return enterAnim.PlayExitCode(bgView, true);
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
            arrow.GetComponent<SpriteRenderer>().color = Color.white;
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
            string text = dialogueModel.Lines[index];
            // 打字
            typingCoroutine = StartCoroutine(TypeLines());
        }
    }
}
