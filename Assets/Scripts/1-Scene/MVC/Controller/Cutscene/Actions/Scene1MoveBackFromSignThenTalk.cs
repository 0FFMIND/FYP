using System.Collections.Generic;
using Manager;
using Utils;

namespace MVC
{
    /// <summary>
    /// 玩家从告示牌后退然后进入对话
    /// </summary>
    public class Scene1MoveBackFromSignThenTalk
    {
        public IEnumerable<IScene1CutsceneCommand> Build()
        {
            yield return new WaitCommand(0.5f);
            yield return new ContextActionCommand(ctx =>
            {
                if (ctx.Mover.anim.currentDir != Direction.Up)
                {
                    ctx.Mover.SetFace(Direction.Up);
                }
            });
            yield return new WaitCommand(0.5f);
            yield return new JumpCommand(0.2f, 0.6f);
            yield return new WaitCommand(0.5f);
            yield return new OffsetMoveCommand(0f, -1.2f, 2f, Direction.Up);
            yield return new WaitCommand(1f);
            yield return new EmotePlayCommand(EmoteType.Thinking, 1f);
            yield return new WaitCommand(1.7f);
            yield return new ContextActionCommand(ctx =>
            {
                ctx.DialogCtl.StartClipDialogue(Scene1DialogueId.SignTutorialEnd, () => TalkEnd(ctx));
            });
        }

        private void TalkEnd(Scene1CutsceneContext ctx)
        {
            if (ctx.GuideCtl != null)
            {
                ctx.GuideCtl.StartDialogue(
                    "1-Scene-6.txt",
                    () =>
                    {
                        // 改变当前scene1状态机状态
                        EventBus.Publish(
                            new EScene1ArrivalPhaseChange(Scene1Phase.AwaitMenuToggle)
                        );
                        PauseMgr.Instance.SetShowGuide(true);
                    }
                );
            }
        }
    }
}
