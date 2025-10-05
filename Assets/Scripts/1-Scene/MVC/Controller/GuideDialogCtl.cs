using System;
using System.Collections;
using UnityEngine;

namespace MVC
{
    public class GuideDialogCtl : DialogCtlBase
    {
        [SerializeField] Animator guideAnim;

        [SerializeField]
        private GameObject root;

        private Action finished;

        private bool startOnce = false;

        private string guideAnimName;

        protected override void OnEnable()
        {
            base.OnEnable();
            root.gameObject.SetActive(false);
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
            guideAnimName = "GuidePanelEnter";
            StartDialogue();
        }

        public void StartSecondDialogue(Action onFinished)
        {
            finished = onFinished;
            modelText = "1-Scene-4.txt";
            guideAnimName = "GuidePanelSecondEnter";
            StartDialogue();
        }

        protected override IEnumerator TypeLines(string fullRaw)
        {
            arrow.gameObject.SetActive(false);

            // 如果是第一句
            if (index == 0 && guideAnimName == "GuidePanelEnter")
            {
                _isEntering = true;
                root.gameObject.SetActive(true);
                int guideLayer = 0;
                guideAnim.gameObject.SetActive(true);
                guideAnim.Play(guideAnimName, guideLayer, 0f);
                yield return new WaitForSeconds(0.5f);
                _isEntering = false;
            }
            else if(index == 0 && !startOnce && guideAnimName == "GuidePanelSecondEnter")
            {
                startOnce = true;
                _isEntering = true;
                root.gameObject.SetActive(true);
                int guideLayer = 0;
                guideAnim.gameObject.SetActive(true);
                guideAnim.Play(guideAnimName, guideLayer, 0f);
                yield return new WaitForSeconds(0.18f);
                _isEntering = false;
            }
            yield return base.TypeLines(fullRaw);
        }

        private IEnumerator PlayClosed()
        {
            string guideAnimName = "GuidePanelExit";
            int guideLayer = 0;
            guideAnim.Play(guideAnimName, guideLayer, 0f);
            yield return new WaitForSeconds(0.2f);
            root.gameObject.SetActive(false);
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
