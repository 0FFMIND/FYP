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
        [Header("ScriptableObject 对话资源")]
        [SerializeField]
        protected string modelText;

        [SerializeField]
        protected LineMapping[] mappings;

        protected Sprite currentSprite;
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

        public void StartDialogue()
        {
            base.StartDialogue(new DialogueModel(modelText));
        }

        protected override IEnumerator TypeLines(Sprite currentSprite = null)
        {
            dialogueRenderer.Hide();
            // 如果是第一句
            if (index == 0)
            {
                _isEntering = true;
                RenderViews(currentSprite, null);
                // 先做文本框上浮
                yield return enterAnim.PlayEnterCode(dialogueRenderer.dialogueView, false);
                _isEntering = false;
            }
            yield return base.TypeLines(currentSprite);
        }

        public void PlayDefaultBGM()
        {
            AudioManager.Instance.PlayBGM("1-bgm");
        }

        public void ArrowRed()
        {
            dialogueRenderer.arrowIndicator.SetColor(Color.red);
        }

        protected override void NextLine()
        {
            dialogueRenderer.arrowIndicator.SetColor(Color.white);
            // 如果读完
            if (index == dialogueModel.Lines.Length)
            {
                EventBus.Publish(new EJournalStepChanged("reachRooftop", 0, StepState.Done));
                AudioManager.Instance.StopBGM(0.5f);
                // 进入1-Scene-Main
                EventBus.Publish(
                    new ESceneFade(
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
            // 打字
            StartCoroutine(TypeLines(currentSprite));
        }
    }
}
