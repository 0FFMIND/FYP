using System.Collections;

namespace MVC
{
    public class JumpCommand : IScene1CutsceneCommand
    {
        private readonly float duration;
        private readonly float height;
        public JumpCommand(float duration, float height) { this.duration = duration; this.height = height; }
        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            ctx.Mover.Jump(duration, height);
            yield break;
        }
    }
}