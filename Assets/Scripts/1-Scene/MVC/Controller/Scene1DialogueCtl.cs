using System.Collections;
using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    public class Scene1DialogueCtl : DialogCtlBase
    {
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

        public override void StartDialogue() => base.StartDialogue();

        protected override IEnumerator TypeLines(string fullRaw)
        {
            arrow.gameObject.SetActive(false);

            // 如果是第一句
            if (index == 0)
            {
                RenderViews(currentSprite, null);
                // 先做文本框上浮
                yield return SlideInDialogueView();
            }
            yield return base.TypeLines(fullRaw);
        }

        protected override void NextLine()
        {
            arrow.GetComponent<SpriteRenderer>().color = Color.white;
            // 如果读完
            if (index == dialogueModel.Lines.Length)
            {
                AudioManager.Instance.StopBGM(0.5f);
                // 进入1-Scene-Main
                EventBus.Publish(
                    new ESceneFadeAdditiveDisable(
                        fromScene: "1-Scene-UI",
                        toScene: "1-Scene-Main",
                        fadeOutDuration: 0.5f,
                        fadeInDuration: 1f
                    )
                );
                // 可以暂停
                PauseMgr.Instance.SetPauseEnabled(true);
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
                        if (eact != Eact.none)
                        {
                            if (eact == Eact.playBGM)
                            {
                                AudioManager.Instance.PlayBGM("1-bgm");
                            }
                            if (eact == Eact.arrowRed)
                            {
                                arrow.GetComponent<SpriteRenderer>().color = Color.red;
                            }
                        }
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
