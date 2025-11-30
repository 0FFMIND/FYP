using System;
using System.Collections;

namespace MVC
{
    /// <summary>
    /// 执行一段基于 Scene1CutsceneContext 的自定义逻辑。
    /// </summary>
    public class ContextActionCommand : IScene1CutsceneCommand
    {
        private readonly Action<Scene1CutsceneContext> _action;

        public ContextActionCommand(Action<Scene1CutsceneContext> action)
        {
            _action = action;
        }

        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            _action?.Invoke(ctx);
            yield break;
        }
    }
}
