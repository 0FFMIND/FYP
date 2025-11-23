using Utils;

namespace MVC
{
    public class Scene1AwaitMenuTogglePhase : IScene1PhaseHandler
    {
        private readonly Scene1PhaseCtl ctl;

        public Scene1AwaitMenuTogglePhase(Scene1PhaseCtl ctl) => this.ctl = ctl;

        public void Enter() { }

        public void OnJournalChanged(EJournalStatusChanged e) { }

        public void OnPauseChanged(EPauseChanged e)
        {
            if (!e.IsPaused)
            {
                ctl.TransitionTo(Scene1Phase.RooftopExplore);
            }
        }

        public void Tick() { }
    }
}
