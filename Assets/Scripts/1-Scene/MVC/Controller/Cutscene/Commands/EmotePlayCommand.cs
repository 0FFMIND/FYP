using System.Collections;

namespace MVC
{
    public class EmotePlayCommand : IScene1CutsceneCommand
    {
        private readonly EmoteType emoteType;
        private readonly float duration;
        public EmotePlayCommand(EmoteType emoteType, float duration) 
        { 
            this.emoteType = emoteType;
            this.duration = duration;
        }
        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            ctx.EmoteCtl.Play(emoteType, duration);
            yield break;
        }
    }
}
