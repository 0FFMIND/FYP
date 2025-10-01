using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    public class TimelineDialogCtl : DialogCtlBase
    {
        [SerializeField]
        private LineMapping[] firstMappings;

        [SerializeField]
        private LineMapping[] secondMappings;

        private Action finished;

        public void HideDialogue()
        {
            // 隐藏内容
            RenderViews(null, null);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public void StartSecondDialogue(Action onFinished)
        {
            mappings = secondMappings;
            finished = onFinished;
            modelText = "1-Scene-2.txt";
            base.StartDialogue();
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
                RenderViews(currentSprite, null);
                bool donePanel = false,
                    doneBG = false;

                // 先做文本框上浮
                IEnumerator RunPanel()
                {
                    yield return SlideInDialogueView();
                    donePanel = true;
                }
                IEnumerator RunBG()
                {
                    yield return SlideInBGView();
                    doneBG = true;
                }

                // 同时开跑
                StartCoroutine(RunPanel());
                StartCoroutine(RunBG());
                yield return new WaitUntil(() => donePanel && doneBG);
            }
            yield return base.TypeLines(fullRaw);
        }

        protected override void NextLine()
        {
            arrow.GetComponent<SpriteRenderer>().color = Color.white;
            // 如果读完
            if (index == dialogueModel.Lines.Length)
            {
                dialogueView.tmp.text = "";
                HideDialogue();
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
