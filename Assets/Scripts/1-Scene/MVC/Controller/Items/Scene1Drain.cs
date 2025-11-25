using UnityEngine;

namespace MVC
{
    public class Scene1Drain : InteractCtl
    {
        [SerializeField]
        private ChoiceCtl gameCtl;

        [SerializeField]
        private ChoiceModel model;
        private bool _used = false; // 只允许交互一次

        // 最简单的购买尝试：判断当前硬币数是否足够，足够则扣除并返回 true，否则返回 false
        public override bool BeginInteract(PlayerCtl player)
        {
            // 切换不同的文本
            visitCount = TryPick();
            return base.BeginInteract(player);
        }

        public void StartDrainGame(InteractCtl ctl)
        {
            ctl?.Done();
            gameCtl.ShowWithClosed(() => { }, model);
        }

        public void PickCanceled()
        {
            visitCount = 1;
            BeginDialogue();
        }

        public int TryPick()
        {
            if (_used)
            {
                // 已经访问过
                return 3;
            }
            // 默认
            return 0;
        }
    }
}
