using Manager;
using UnityEngine;
using Utils;

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

        // 对话步骤编号（与 InteractModel 配置对应）
        private const int STEP_NO_COIN = 0; // 没有硬币，提示无法购买
        private const int STEP_HAS_COIN_CHOICE = 2; // 有硬币，弹出是否购买的选项
        private const int STEP_ALREADY_USED = 3; // 已经使用过，给“用过了”的台词
        private const int STEP_CANCELLED = 4; // 选择不买 / 取消购买
        private const int STEP_SUCCESS = 5; // 购买成功
        private const int STEP_UI_CUTSCENE = 6; // 进入 UI 过场的对话
        private const int STEP_INSUFFICIENT_COINS = 7; // 硬币不够

        // 最简单的购买尝试：判断当前硬币数是否足够，足够则扣除并返回 true，否则返回 false
        public override bool BeginInteract(PlayerCtl player, bool shouldEndInteract = true)
        {
            // 切换不同的文本
            visitCount = TryVend();
            if (visitCount == STEP_HAS_COIN_CHOICE)
            {
                // 点击选项后取消结束交互的回调，暂不结束交互
                return base.BeginInteract(player, false);
            }
            return base.BeginInteract(player);
        }

        public void TryPublishJournal(InteractCtl ctl)
        {
            EventBus.Publish(new EJournalStatusChanged("vendingMachine", JournalStatus.Active));
            ctl?.Done();
        }

        public void VendSuccess()
        {
            int coins = InventoryMgr.Instance.GetCountById(coinItemId); // 查询当前背包中此硬币的总数

            if (coins < price)
            {
                visitCount = STEP_INSUFFICIENT_COINS;
                ContinueDialogue();
            }
            else
            {
                // 投入硬币
                InventoryMgr.Instance.TryConsumeById(coinItemId, price);
                AudioMgr.Instance.PlaySFX("coin");
                visitCount = STEP_SUCCESS;
                _used = true;
                isCompleted = true;
                ContinueDialogue();
            }
        }

        public void StartVendCutScene()
        {
            // 等 EnterUI 完成后再切 visitCount/开始对话
            switcher.EnterUI(() =>
            {
                AudioMgr.Instance.PlayBGM("1-bgm-2", 0f);
                TimelineCtl = dialogCtl;
                dialogCtl = UICtl;
                visitCount = STEP_UI_CUTSCENE;
                ContinueDialogue();
            });
        }

        public void EndVendCutScene(InteractCtl ctl)
        {
            SettingsMgr.Instance.SetChapter1HiddenCompleted(true);
            switcher.ExitUI(() =>
            {
                AudioMgr.Instance.PlayBGM("1-bgm-1", 0f);
                dialogCtl = TimelineCtl;
                ctl?.Done();
            });
        }

        public void VendCanceled()
        {
            visitCount = STEP_CANCELLED;
            ContinueDialogue();
        }

        public int TryVend()
        {
            if (_used)
            {
                return STEP_ALREADY_USED;
            }

            // 获取全局背包管理器实例
            var mgr = InventoryMgr.Instance;
            // 查询当前背包中此硬币的总数
            int coins = mgr.GetCountById(coinItemId);

            if (coins == 0)
            {
                // 若没有硬币
                return STEP_NO_COIN;
            }
            return STEP_HAS_COIN_CHOICE;
        }
    }
}
