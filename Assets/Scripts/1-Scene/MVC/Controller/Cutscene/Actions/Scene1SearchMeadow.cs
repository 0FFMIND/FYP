using System.Collections.Generic;
using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    /// <summary>
    /// 玩家在草甸上搜寻种子的过场命令序列
    /// </summary>
    public class Scene1SearchMeadow
    {

        public IEnumerable<IScene1CutsceneCommand> Build()
        {
            var rightX = 0f;
            var leftX = 0f;
            var stepX = 0f;
            var x = 0f;
            yield return new ContextActionCommand(ctx =>
            {
                var t = ctx.Mover.transform;
                x = t.position.x;
                rightX = ctx.RightX;
                stepX = ctx.StepX;
                leftX = ctx.LeftX;
            });
            // 向右移动到指定右边X位置
            // 如果当前在右边界之外 ⇒ 不去 rightX，直接去 leftX → finalX
            if (x < rightX - stepX)
            {
                yield return new StepMoveCommand(rightX, false);
            }
            // 再向左移动到最左边
            yield return new StepMoveCommand(leftX, true);
            // 再向右到最终位置
            yield return new ContextActionCommand(ctx =>
            {
                var target = new Vector3(
                    ctx.Mover.transform.position.x + ctx.StepX,
                    ctx.Mover.transform.position.y,
                    ctx.Mover.transform.position.z
                );
                ctx.Mover.StartMove(target, 2.3f, Direction.Right, null);
            });
            yield return new WaitCommand(0.5f);
            yield return new SetFaceCommand(Direction.Up);
            yield return new WaitCommand(0.1f);
            yield return new JumpCommand(0.2f, 0.6f);
            yield return new EmotePlayCommand(EmoteType.Warning, 0.6f, true);
            yield return new WaitCommand(1f);
            yield return new ContextActionCommand(ctx =>
            {
                ctx.DialogCtl.StartClipDialogue(Scene1DialogueId.MeadowExploreMid, () =>
                {
                    EventBus.Publish(new EJournalStepChanged("endRooftop", 1, StepState.Done));
                    ctx.Switcher.EnterUI(() =>
                    {
                        AudioMgr.Instance.PlayBGM("1-bgm-2", 0f);
                        ctx.UICtl.StartClipDialogue(Scene1DialogueId.MeadowExploreEnd, () =>
                        {
                            ctx.Switcher.ExitUI(() =>
                            {
                                ctx.CutsceneCtl.RunAwayIntro();
                            });
                        });
                    });
                });
            });
        }
    }
}
