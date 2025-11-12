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
            // ÐÞ¸Äjournal
            EventBus.Publish(new EJournalStepChanged("reachRooftop", 1, StepState.Done));

            var it = JournalMgr.Instance?.Model?.Find("exploreRooftop");
            if (it != null && it.status != JournalStatus.Completed)
            {
                EventBus.Publish(new EJournalStatusChanged("exploreRooftop", JournalStatus.Active));
            }
            // ´¥·¢guide
            EventBus.Publish(new EScene1ArrivalStateChange(Scene1State.MenuTutorial));
            ctl?.Done();
        }

    }

}
