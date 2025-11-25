using System;
using System.Collections;
using UnityEngine;

namespace MVC
{
    public class InventoryDialogCtl : DialogCtlBase
    {
        [SerializeField]
        EnterAnim enterAnim;

        private Action finished;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }


        public void StartDialogue(string[] lines, Action onFinished)
        {
            dialogueModel = new DialogueModel(lines);
            finished = onFinished;
            StartDialogue(new DialogueModel(lines));
        }

        protected override IEnumerator TypeLines(Sprite currentSprite = null)
        {
            // 如果是第一句
            if (index == 0)
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
            // 如果读完
            if (index == dialogueModel.Lines.Length)
            {
                // 清空文本
                dialogueRenderer.Hide();
                // 播放动画
                StartCoroutine(PlayClosed());
            }
            // 不然按钮点击会误认为nextline
            if (dialogueModel == null || index >= dialogueModel.Lines.Length)
            {
                return;
            }
            // 打字
            StartCoroutine(TypeLines());
        }
    }
}
