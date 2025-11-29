using System.Collections;
using System.Collections.Generic;

namespace MVC
{
    /// <summary>
    /// 纯 C# 的场景过场管理类，不依赖 MonoBehaviour。
    /// 因此可以通过 runner = new Scene1CutsceneRunner() 创建使用，
    /// 而无需挂在 GameObject 上或使用 AddComponent<Scene1CutsceneRunner>();创建
    /// </summary>
    public class Scene1CutsceneRunner
    {
        public IEnumerator Run(Scene1CutsceneContext ctx, IEnumerable<IScene1CutsceneCommand> cmds)
        {
            foreach (var c in cmds)
                yield return c.Execute(ctx);
        }
    }
}