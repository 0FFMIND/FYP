using Utils;

namespace MVC
{
    public class Scene1MenuTutorialPhase : IScene1PhaseHandler
    {
        private readonly Scene1PhaseCtl ctl;

        public Scene1MenuTutorialPhase(Scene1PhaseCtl ctl) => this.ctl = ctl;

        public void Enter() {
            // 禁止玩家移动
            ctl.Player.model.SetDisabled(true);
            // 开始播放动画
            ctl.CutsceneCtl.MoveBackFromSignThenTalk();
        }

        public void OnJournalChanged(EJournalStatusChanged e) { }

        public void OnPauseChanged(EPauseChanged e) { }

        public void Tick() { }
    }
}
