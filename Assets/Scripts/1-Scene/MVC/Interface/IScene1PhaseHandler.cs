using Utils;

namespace MVC
{
    public interface IScene1PhaseHandler
    {
        void Enter();
        void Tick(); // 可空实现
        void OnJournalChanged(EJournalStatusChanged e);
        void OnPauseChanged(EPauseChanged e);
    }
}
