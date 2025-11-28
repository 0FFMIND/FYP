using Manager;
using Utils;

namespace MVC
{
    public class Scene1RooftopExplorePhase : IScene1PhaseHandler
    {
        private readonly Scene1PhaseCtl ctl;

        private enum RooftopExploreStage
        {
            StartRooftopExplore,
            AwaitExploreComplete,
            Done,
        }

        private RooftopExploreStage rooftopExploreStage;

        public Scene1RooftopExplorePhase(Scene1PhaseCtl ctl) => this.ctl = ctl;

        public void Enter()
        {
            rooftopExploreStage = RooftopExploreStage.StartRooftopExplore;
            ctl.Player.model.SetDisabled(true);
            ctl.TimelineCtl.StartDialogue(Scene1DialogueId.RooftopExploreIntro, () =>
            {
                rooftopExploreStage = RooftopExploreStage.AwaitExploreComplete;
                ctl.Player.model.SetDisabled(false);
                var it = JournalMgr.Instance?.Model?.Find("exploreRooftop");
                if (it != null && it.status == JournalStatus.Completed)
                {
                    // 已完成则直接推进到 ExploreCompleted
                    rooftopExploreStage = RooftopExploreStage.Done;
                    EnterExploreCompleted();
                    return;
                }
            });
        }

        private void EnterExploreCompleted()
        {
            if (rooftopExploreStage != RooftopExploreStage.Done)
            {
                return;
            }
            ctl.Player.model.SetDisabled(true);
            // 开始播放动画
            ctl.StartCoroutine(ctl.Mover.PlayerFourthMove());
        }

        public void OnJournalChanged(EJournalStatusChanged e)
        {
            if (
                e.Key == "exploreRooftop"
                && e.NewStatus == JournalStatus.Completed
                && rooftopExploreStage == RooftopExploreStage.AwaitExploreComplete
            )
            {
                rooftopExploreStage = RooftopExploreStage.Done;
                // 进入到下一状态
                EnterExploreCompleted();
            }
        }

        public void OnPauseChanged(EPauseChanged e) { }

        public void Tick() { }
    }
}
