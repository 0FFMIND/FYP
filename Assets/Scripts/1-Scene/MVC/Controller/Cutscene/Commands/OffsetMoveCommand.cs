using System.Collections;

namespace MVC
{
    /// <summary>
    /// 按给定的 (deltaX, deltaY) 偏移一次移动。
    /// 只负责触发移动，不等待移动完成。
    /// </summary>
    public class OffsetMoveCommand : IScene1CutsceneCommand
    {
        private readonly float deltaX;
        private readonly float deltaY;
        private readonly float duration;
        private readonly Direction direction; // 面朝方向，由调用方决定

        public OffsetMoveCommand(float deltaX, float deltaY, float duration, Direction direction)
        {
            this.deltaX = deltaX;
            this.deltaY = deltaY;
            this.duration = duration;
            this.direction = direction;
        }

        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            var pos = ctx.Mover.transform.position;
            pos.x += deltaX;
            pos.y += deltaY;
            ctx.Mover.StartMove(pos, duration, direction, null);
            yield break;
        }
    }
}
