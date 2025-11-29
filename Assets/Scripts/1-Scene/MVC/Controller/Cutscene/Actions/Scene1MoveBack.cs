using System.Collections;
using UnityEngine;

namespace MVC
{
    /// <summary>
    /// 按给定的向上偏移量和时长，启动一次向上的 MoveBack 移动。
    /// 只负责触发移动，不等待移动完成。
    /// </summary>
    public class MoveBackCommand : IScene1CutsceneCommand
    {
        private readonly float deltaY;
        private readonly float duration;

        public MoveBackCommand(float deltaY, float duration)
        {
            this.deltaY = deltaY;
            this.duration = duration;
        }

        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            var pos = ctx.mover.transform.position;
            pos.y -= deltaY;

            // 仅触发移动，不阻塞
            ctx.mover.StartMove(pos, duration, Direction.Up, null);

            yield break;
        }
    }
}
