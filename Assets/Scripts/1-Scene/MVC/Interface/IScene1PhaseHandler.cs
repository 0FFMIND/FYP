using Utils;

namespace MVC
{
    public interface IScene1PhaseHandler
    {
        void Enter();
        void Tick(); // ø…ø’ µœ÷
        void OnJournalChanged(EJournalStatusChanged e);
        void OnPauseChanged(EPauseChanged e);
    }
}
