using System;
using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    public class Scene1Stone : MonoBehaviour
    {
        [SerializeField]
        InventoryDialogCtl dialogCtl;

        [SerializeField]
        string[] lines;

        [SerializeField]
        bool isRight;

        [SerializeField]
        bool isDrain;

        public void AddCoin(InteractCtl ctl)
        {
            InventoryMgr.Instance.AddById("coin", 1);
            dialogCtl.StartDialogue(
                lines,
                () =>
                {
                    // 通知 InteractCtl 可以收尾
                    ctl?.Done();
                    if (isDrain)
                    {
                        EventBus.Publish(
                            new EJournalStepChanged("vendingMachine", 0, StepState.Done)
                        );
                    }
                    else
                    {
                        if (isRight)
                        {
                            EventBus.Publish(
                                new EJournalStepChanged("vendingMachine", 2, StepState.Done)
                            );
                        }
                        else
                        {
                            EventBus.Publish(
                                new EJournalStepChanged("vendingMachine", 1, StepState.Done)
                            );
                        }
                    }
                }
            );
        }
    }
}
