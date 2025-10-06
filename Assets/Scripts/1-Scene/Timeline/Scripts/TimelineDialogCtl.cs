using System;
using System.Collections;
using UnityEngine;

namespace MVC
{
    public class TimelineDialogCtl : DialogCtlBase
    {
        [SerializeField] EnterAnim enterAnim;

        [SerializeField]
        private LineMapping[] firstMappings;

        [SerializeField]
        private LineMapping[] secondMappings;

        [SerializeField]
        private GameObject dialogPanel;

        private Action finished;

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

        public void StartInteractDialogue(Action onFinished)
        {
            mappings = secondMappings;
            finished = onFinished;
            modelText = "1-Scene-2.txt";
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

        protected override IEnumerator TypeLines(string fullRaw)
        {
            arrow.gameObject.SetActive(false);

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
                IEnumerator RunBG()
                {
                    yield return enterAnim.PlayEnterCode(bgView, true);
                    doneBG = true;
                }

                // 同时开跑
                StartCoroutine(RunPanel());
                StartCoroutine(RunBG());
                yield return new WaitUntil(() => donePanel && doneBG);
                _isEntering = false;
            }
            yield return base.TypeLines(fullRaw);
        }

        protected override void NextLine()
        {
            arrow.GetComponent<SpriteRenderer>().color = Color.white;
            // 如果读完
            if (index == dialogueModel.Lines.Length)
            {
                End();
                // 清空文本
                dialogueView.tmp.text = "";
                // 隐藏对话与背景
                HideDialogue();
                dialogPanel.gameObject.SetActive(false);
                // 关掉向下小箭头
                arrow.gameObject.SetActive(false);
                // 触发对话结束回调
                finished?.Invoke();
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
                    foreach (Eact eact in map.eacts)
                    {
                        if (eact != Eact.none) { }
                    }

                    break;
                }
            }
            string text = dialogueModel.Lines[index];
            // 打字
            typingCoroutine = StartCoroutine(TypeLines(text));
            // 移动到下一个line
            index++;
        }
    }
}
