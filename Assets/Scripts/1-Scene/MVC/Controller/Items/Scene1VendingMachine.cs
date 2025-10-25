using Manager;
using UnityEngine;

namespace MVC
{
    public class Scene1VendingMachine : InteractCtl
    {
        [SerializeField]
        CameraSwitch switcher;

        [SerializeField]
        TimelineDialogCtl UICtl;
        private TimelineDialogCtl TimelineCtl;

        private int price = 3; // 价格：需要的硬币数量
        private string coinItemId = "coin"; // 硬币物品的 id
        private bool _used = false; // 只允许交互一次

        // 最简单的购买尝试：判断当前硬币数是否足够，足够则扣除并返回 true，否则返回 false
        public override bool BeginInteract(PlayerCtl player)
        {
            // 切换不同的文本
            visitCount = TryVend();
            return base.BeginInteract(player);
        }

        public void VendSuccess()
        {
            int coins = InventoryMgr.Instance.GetCountById(coinItemId); // 查询当前背包中此硬币的总数

            if (coins < price)
            {
                visitCount = 7;
                BeginDialogue();
            }
            else
            {
                // 投入硬币
                InventoryMgr.Instance.TryConsumeById(coinItemId, price);
                AudioManager.Instance.PlaySFX("coin");
                visitCount = 5;
                _used = true;
                BeginDialogue();
            }
        }

        public void StartVendCutScene()
        {
            // 等 EnterUI 完成后再切 visitCount/开始对话
            switcher.EnterUI(() =>
            {
                AudioManager.Instance.PlayBGM("1-bgm-2", 0f);
                TimelineCtl = dialogCtl;
                dialogCtl = UICtl;
                visitCount = 6;
                BeginDialogue();
            });
        }

        public void EndVendCutScene(InteractCtl ctl)
        {
            switcher.ExitUI(() =>
            {
                AudioManager.Instance.PlayBGM("1-bgm-1", 0f);
                dialogCtl = TimelineCtl;
                ctl?.Done();
            });
        }

        public void VendCanceled()
        {
            visitCount = 4;
            BeginDialogue();
        }

        public int TryVend()
        {
            if (_used)
            {
                return 3;
            }
            var mgr = InventoryMgr.Instance; // 获取全局背包管理器实例

            int coins = mgr.GetCountById(coinItemId); // 查询当前背包中此硬币的总数
            if (coins == 0)
            {
                // 若没有硬币
                return 0;
            }
            return 2;
        }
    }
}
