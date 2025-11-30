using UnityEngine;
using Utils;

namespace MVC
{
    public class Scene1MeadowExplorePhase : IScene1PhaseHandler
    {
        private readonly Scene1PhaseCtl ctl;

        private enum MeadowExploreStage
        {
            StartMeadowExplore,
            AwaitExploreComplete,
        }

        private MeadowExploreStage meadowExploreStage;
        private bool interactMeadowOnce = false;

        public Scene1MeadowExplorePhase(Scene1PhaseCtl ctl) => this.ctl = ctl;

        public void Enter()
        {
            meadowExploreStage = MeadowExploreStage.StartMeadowExplore;
            ctl.Player.model.SetDisabled(false);
        }

        private void EnterInteractMeadow()
        {
            // 禁止玩家移动
            ctl.Player.model.SetDisabled(true);
            // 开始播放动画
            // ctl.StartCoroutine(ctl.CutsceneCtl.PlayerFifthMove());
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
                && target.gameObject.name == "meadow"
                && !interactMeadowOnce
                && meadowExploreStage == MeadowExploreStage.StartMeadowExplore
            )
            {
                meadowExploreStage = MeadowExploreStage.AwaitExploreComplete;
                interactMeadowOnce = true;
                EnterInteractMeadow();
            }
        }
    }
}
