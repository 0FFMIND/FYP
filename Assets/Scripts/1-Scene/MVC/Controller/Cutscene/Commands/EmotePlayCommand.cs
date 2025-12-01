using System.Collections;

namespace MVC
{
    public class EmotePlayCommand : IScene1CutsceneCommand
    {
        private readonly EmoteType emoteType;
        private readonly float duration;

        private readonly bool skipIn;
        public EmotePlayCommand(EmoteType emoteType, float duration, bool skipIn = false) 
        { 
            this.emoteType = emoteType;
            this.duration = duration;
            this.skipIn = skipIn;
        }
        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            ctx.EmoteCtl.Play(emoteType, duration, skipIn);
            yield break;
        }
    }
}
