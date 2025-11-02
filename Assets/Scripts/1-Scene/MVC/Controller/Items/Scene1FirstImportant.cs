
using UnityEngine;
using Utils;

namespace MVC
{
    public class Scene1FirstImportant : MonoBehaviour
    {
        [SerializeField] private bool isMeadow;
        [SerializeField] private bool isCleaning;
        [SerializeField] private bool isFlowerPot;
        public void PublishJournal(InteractCtl ctl)
        {
            if (isMeadow)
            {
                EventBus.Publish(new EJournalStepChanged("exploreRooftop", 0, StepState.Done));
            }else if (isCleaning)
            {
                EventBus.Publish(new EJournalStepChanged("exploreRooftop", 2, StepState.Done));
            }else if (isFlowerPot)
            {
                EventBus.Publish(new EJournalStepChanged("exploreRooftop", 1, StepState.Done));
            }
            ctl?.Done();
        }
    }
}
