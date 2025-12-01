using System.Collections;
using UnityEngine;
using Utils;

namespace MVC
{
    public class Scene1SignTutorialPhase : IScene1PhaseHandler
    {
        private readonly Scene1PhaseCtl ctl;
        private bool interactSignOnce = false;

        private enum SignTutorialStage
        {
            StartSignGuide,
            AwaitSignInteract,
            Done,
        }

        private SignTutorialStage signTutorialStage;

        public Scene1SignTutorialPhase(Scene1PhaseCtl ctl) => this.ctl = ctl;

        public void Enter()
        {
            signTutorialStage = SignTutorialStage.StartSignGuide;

            if (ctl.GuideCtl != null)
            {
                ctl.GuideCtl.StartDialogue(
                    "1-Scene-3.txt",
                    () =>
                    {
                        signTutorialStage = SignTutorialStage.AwaitSignInteract;
                        // 玩家可以移动
                        ctl.Player.model.SetDisabled(false);
                        // 场景中出现引导脚印
                        ctl.GuideSteps.SetActive(true);
                    }
                );
            }
        }

        public void OnJournalChanged(EJournalStatusChanged e) { }

        public void OnPauseChanged(EPauseChanged e) { }

        public void Tick()
        {
            var current = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<PlayerInteractCtl>()
                .target;
            var target = (current as Component)?.gameObject;
            if (
                target != null
                && target.gameObject.name == "metalSign"
                && !interactSignOnce
                && signTutorialStage == SignTutorialStage.AwaitSignInteract
            )
            {
                signTutorialStage = SignTutorialStage.Done;
                interactSignOnce = true;
                ctl.StartCoroutine(InteractSign());
            }
        }

        private IEnumerator InteractSign()
        {
            // 禁止玩家移动
            ctl.Player.model.SetDisabled(true);
            // 播放人物思考
            var emoteCtl = ctl.Player.gameObject.GetComponent<PlayerEmoteCtl>();
            emoteCtl.Play(EmoteType.Thinking, 1f);
            yield return new WaitForSecondsRealtime(1.5f);
            // 播放guide
            ctl.GuideCtl.StartDialogue(
                "1-Scene-4.txt",
                () =>
                {
                    // 关闭引导脚印
                    ctl.GuideSteps.SetActive(false);
                    // 移动人物
                    ctl.Player.model.SetDisabled(false);
                    // 重新显示交互图标
                    ctl.Player.gameObject.GetComponent<PlayerInteractCtl>().Refresh();
                }
            );
        }
    }
}
