using System.Collections;
using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    public class Scene1PreArrivalDialogCtl : DialogCtlBase
    {
        [SerializeField]
        EnterAnim enterAnim;

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
                _isEntering = true;
                RenderViews(currentSprite, null);
                // 先做文本框上浮
                yield return enterAnim.PlayEnterCode(dialogueView, false);
                _isEntering = false;
            }
            yield return base.TypeLines(fullRaw);
        }

        public void PlayDefaultBGM()
        {
            AudioManager.Instance.PlayBGM("1-bgm");
        }

        public void ArrowRed()
        {
            arrow.GetComponent<SpriteRenderer>().color = Color.red;
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
                        fadeInDuration: 2f
                    )
                );
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
            typingCoroutine = StartCoroutine(TypeLines(text));
            // 移动到下一个line
            index++;
        }
    }
}
