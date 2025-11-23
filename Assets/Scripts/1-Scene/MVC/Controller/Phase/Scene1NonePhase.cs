using Utils;

namespace MVC
{
    public class Scene1NonePhase : IScene1PhaseHandler
    {
        private readonly Scene1PhaseCtl ctl;

        public Scene1NonePhase(Scene1PhaseCtl ctl) => this.ctl = ctl;

        public void Enter()
        {
            ctl.Player.model.SetDisabled(false);
        }

        public void OnJournalChanged(EJournalStatusChanged e) { }

        public void OnPauseChanged(EPauseChanged e) { }

        public void Tick() { }
    }
}
