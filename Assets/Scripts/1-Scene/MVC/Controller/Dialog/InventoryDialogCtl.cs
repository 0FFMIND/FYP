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

        public override void StartDialogue()
        {
            base.StartDialogue();
        }

        public void StartDialogue(string[] lines, Action onFinished)
        {
            dialogueModel = new DialogueModel(lines);
            finished = onFinished;
            StartDialogue();
        }

        protected override IEnumerator TypeLines()
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
                index++;
                // 清空文本
                dialogueView.tmp.text = "";
                HideArrow();
                // 播放动画
                StartCoroutine(PlayClosed());
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
                    break;
                }
            }
            // 打字
            StartCoroutine(TypeLines());
        }
    }
}
