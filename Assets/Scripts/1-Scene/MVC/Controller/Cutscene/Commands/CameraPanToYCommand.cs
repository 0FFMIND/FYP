using System;
using System.Collections;

namespace MVC
{
    public class CameraPanToYCommand : IScene1CutsceneCommand
    {
        private readonly float duration;
        private readonly float targetY;

        public CameraPanToYCommand(float targetY, float duration)
        {
            this.targetY = targetY;
            this.duration = duration;
        }

        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            ctx.CameraCtl.PanToY(targetY, duration);
            yield break;
        }
    }
}
