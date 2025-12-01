using UnityEngine;
using Utils;

namespace MVC
{
    public class Scene1AwaitMenuTogglePhase : IScene1PhaseHandler
    {
        private readonly Scene1PhaseCtl ctl;
        private bool hasShownMenuMoveGuide = false;
        public Scene1AwaitMenuTogglePhase(Scene1PhaseCtl ctl) => this.ctl = ctl;

        public void Enter() { }

        public void OnJournalChanged(EJournalStatusChanged e) { }

        public void OnPauseChanged(EPauseChanged e)
        {
            if (!e.IsPaused)
            {
                ctl.TransitionTo(Scene1Phase.RooftopExplore);
            }
        }

        public void Tick()
        {

            // 硬编码：在 MenuTutorial 阶段，如果玩家按下 WASD 或方向键，就弹出 Guide 提示
            if (!hasShownMenuMoveGuide)
            {
                if (Input.GetKeyDown(KeyCode.W)
                    || Input.GetKeyDown(KeyCode.A)
                    || Input.GetKeyDown(KeyCode.S)
                    || Input.GetKeyDown(KeyCode.D)
                    || Input.GetKeyDown(KeyCode.UpArrow)
                    || Input.GetKeyDown(KeyCode.DownArrow)
                    || Input.GetKeyDown(KeyCode.LeftArrow)
                    || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    hasShownMenuMoveGuide = true;

                    // 简单弹一行引导：请先按 ESC 键打开菜单
                    if (ctl.GuideCtl != null)
                    {
                        ctl.GuideCtl.StartDialogue("1-Scene-6.5.txt", () =>
                        {
                            hasShownMenuMoveGuide = false;
                        });
                    }
                }
            }
        }
    }
}
