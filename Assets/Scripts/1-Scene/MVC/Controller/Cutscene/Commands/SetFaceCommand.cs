using System.Collections;

namespace MVC
{
    public class SetFaceCommand : IScene1CutsceneCommand
    {
        private readonly Direction direction;
        public SetFaceCommand(Direction direction) { this.direction = direction; }
        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            ctx.Mover.SetFace(direction);
            yield break;
        }
    }
}