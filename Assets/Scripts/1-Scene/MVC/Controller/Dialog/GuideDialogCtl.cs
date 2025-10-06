using System;
using System.Collections;
using UnityEngine;

namespace MVC
{
    public class GuideDialogCtl : DialogCtlBase
    {
        [SerializeField] EnterAnim enterAnim;

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

        public void StartFirstDialogue(Action onFinished)
        {
            finished = onFinished;
            modelText = "1-Scene-3.txt";
            StartDialogue();
        }

        public void StartSecondDialogue(Action onFinished)
        {
            finished = onFinished;
            modelText = "1-Scene-4.txt";
            StartDialogue();
        }

        protected override IEnumerator TypeLines(string fullRaw)
        {
            arrow.gameObject.SetActive(false);

            // 如果是第一句
            if (index == 0 && startOnce)
            {
                _isEntering = true;
                yield return enterAnim.PlayEnterAnim("GuidePanelEnter", 0.5f);
                startOnce = false;
                _isEntering = false;
            }
            else if(index == 0 && !startOnce)
            {
                _isEntering = true;
                yield return enterAnim.PlayEnterAnim();
                _isEntering = false;
            }
            yield return base.TypeLines(fullRaw);
        }

        private IEnumerator PlayClosed()
        {
            yield return enterAnim.PlayExit();
            // 触发对话结束回调
            finished?.Invoke();
        }

        protected override void NextLine()
        {
            arrow.GetComponent<SpriteRenderer>().color = Color.white;
            // 如果读完
            if (index == dialogueModel.Lines.Length)
            {
                // 清空文本
                dialogueView.tmp.text = "";
                // 关掉向下小箭头
                arrow.gameObject.SetActive(false);
                // 关闭订阅
                Unsubscribe();
                // 播放动画
                StartCoroutine(PlayClosed());
            }
            // 不然按钮点击会误认为nextline
            if (dialogueModel == null || index >= dialogueModel.Lines.Length)
            {
                return;
            }
            string text = dialogueModel.Lines[index];
            // 打字
            typingCoroutine = StartCoroutine(TypeLines(text));
            // 移动到下一个line
            index++;
        }
    }
}
