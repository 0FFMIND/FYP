using System;
using Manager;
using UnityEngine;

namespace MVC
{
    public class Scene1Stone : MonoBehaviour
    {
        [SerializeField]
        InventoryDialogCtl dialogCtl;

        [SerializeField]
        string[] lines;

        public void AddCoin(InteractCtl ctl)
        {
            InventoryMgr.Instance.AddById("coin", 1);
            dialogCtl.StartDialogue(
                lines,
                () =>
                {
                    // 通知 InteractCtl 可以收尾
                    ctl?.Done();
                }
            );
        }
    }
}
