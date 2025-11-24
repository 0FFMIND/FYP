using System;
using System.Collections;
using UnityEngine;

namespace MVC
{
    public class GuideDialogCtl : DialogCtlBase
    {
        [SerializeField]
        EnterAnim enterAnim;

        private Action finished;

        private bool startOnce = true;

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

        public void StartDialogue(string modelText, Action onFinished)
        {
            finished = onFinished;
            this.modelText = modelText;
            StartDialogue();
        }

        public void StartDialogue(string[] text, Action onFinished)
        {
            finished = onFinished;
            this.dialogueModel = new DialogueModel(text);
            this.modelText = "";
            StartDialogue();
        }

        protected override IEnumerator TypeLines()
        {
            arrowIndicator.Hide();

            // 如果是第一句
            if (index == 0 && startOnce)
            {
                _isEntering = true;
                yield return enterAnim.PlayEnterAnim("GuidePanelEnter", 1.1f);
                startOnce = false;
                _isEntering = false;
            }
            else if (index == 0 && !startOnce)
            {
                _isEntering = true;
                yield return enterAnim.PlayEnterAnim();
                _isEntering = false;
            }
            yield return base.TypeLines();
        }

        private IEnumerator PlayClosed()
        {
            yield return enterAnim.PlayExit();
            // 触发对话结束回调
            finished?.Invoke();
        }

        protected override void NextLine()
        {
            arrowIndicator.SetColor(Color.white);
            // 如果读完
            if (index == dialogueModel.Lines.Length)
            {
                // 清空文本
                dialogueView.tmp.text = "";
                // 关掉向下小箭头
                arrowIndicator.Hide();
                // 播放动画
                StartCoroutine(PlayClosed());
            }
            // 不然按钮点击会误认为nextline
            if (dialogueModel == null || index >= dialogueModel.Lines.Length)
            {
                return;
            }
            // 打字
            typingCoroutine = StartCoroutine(TypeLines());
        }
    }
}
