using System.Collections.Generic;
using Utils;

namespace MVC
{
    /// <summary>
    /// 玩家环顾四周然后进入对话
    /// </summary>
    public class Scene1LookAroundThenTalk
    {
        public IEnumerable<IScene1CutsceneCommand> Build()
        {
            yield return new SetFaceCommand(Direction.Down);
            yield return new WaitCommand(0.4f);
            yield return new OffsetMoveCommand(0f, -0.7f, 1.6f, Direction.Down);
            yield return new WaitCommand(1f);
            yield return new SetFaceCommand(Direction.Right);
            yield return new WaitCommand(1f);
            yield return new SetFaceCommand(Direction.Left);
            yield return new WaitCommand(1f);
            yield return new SetFaceCommand(Direction.Down);
            yield return new WaitCommand(1f);
            yield return new EmotePlayCommand(EmoteType.Thinking, 1f);
            yield return new WaitCommand(1.7f);
            yield return new ContextActionCommand(ctx =>
            {
                // 暂停director
                ctx.Director.Pause();
                ctx.DialogCtl.StartClipDialogue(
                    Scene1DialogueId.TimelineEnd,
                    () =>
                    {
                        // 停止Timeline过场动画
                        ctx.Director.Stop();
                        // 改变当前scene1状态机状态
                        EventBus.Publish(
                            new EScene1ArrivalPhaseChange(Scene1Phase.SignTutorial)
                        );
                    });
            });
        }
    }
}
