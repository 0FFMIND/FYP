using System.Collections;

namespace MVC
{
    public class SetPlayerCloseDoorSpriteCommand : IScene1CutsceneCommand
    {
        private readonly int spriteIndex;
        public SetPlayerCloseDoorSpriteCommand(int spriteIndex) { this.spriteIndex = spriteIndex; }
        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            ctx.Mover.SetSprite(ctx.CloseDoorSprites[spriteIndex]);
            yield break;
        }
    }
}