using System.Collections;
using Manager;
using UnityEngine;

namespace MVC
{
    // 按固定步长把玩家在水平线上推进到 targetX；
    // 自动设置朝向并等待每步动画完成
    public class StepMoveCommand : IScene1CutsceneCommand
    {
        private readonly float targetX;
        private readonly bool isLeft;
        public StepMoveCommand(float targetX, bool isLeft) { this.targetX = targetX; this.isLeft = isLeft; }
        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            var current = ctx.Mover.transform.position.x;
            while (Mathf.Abs(targetX - current) > ctx.StepX)
            {
                var target = new Vector3(
                    isLeft
                        ? ctx.Mover.transform.position.x - ctx.StepX
                        : ctx.Mover.transform.position.x + ctx.StepX,
                    ctx.Mover.transform.position.y,
                    ctx.Mover.transform.position.z
                );
                ctx.Mover.StartMove(target, 2.3f, isLeft ? Direction.Left : Direction.Right, null);
                yield return new WaitForSecondsRealtime(0.5f);
                ctx.Mover.SetFace(Direction.Up);
                yield return new WaitForSecondsRealtime(0.05f);
                AudioMgr.Instance.PlaySFX("warning");
                ctx.EmoteCtl.Play(EmoteType.Error, 0.6f, true);
                yield return new WaitForSecondsRealtime(1f);
                current = ctx.Mover.transform.position.x;
            }
        }
    }
}