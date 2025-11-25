using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    public class Scene1MetalSign : MonoBehaviour
    {
        [SerializeField]
        GuideDialogCtl dialogCtl;

        [SerializeField]
        string[] lines;

        public void PublishJournal(InteractCtl ctl)
        {
            // 修改journal
            EventBus.Publish(new EJournalStepChanged("reachRooftop", 1, StepState.Done));

            var it = JournalMgr.Instance?.Model?.Find("exploreRooftop");
            if (it != null && it.status != JournalStatus.Completed)
            {
                EventBus.Publish(new EJournalStatusChanged("exploreRooftop", JournalStatus.Active));
            }
            // 触发guide
            EventBus.Publish(new EScene1ArrivalPhaseChange(Scene1Phase.MenuTutorial));
            ctl?.Done();
        }

    }

}
