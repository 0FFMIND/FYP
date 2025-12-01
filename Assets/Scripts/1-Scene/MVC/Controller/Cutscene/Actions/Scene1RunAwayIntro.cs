using System.Collections.Generic;
using Manager;

namespace MVC
{
    /// <summary>
    /// 玩家准备逃跑这一小段过场的命令序列
    /// </summary>
    public class Scene1RunAwayIntro
    {

        public IEnumerable<IScene1CutsceneCommand> Build()
        {
            yield return new WaitCommand(0.1f);
            yield return new EmotePlayCommand(EmoteType.Warning, 0.6f, true);
            yield return new JumpCommand(0.2f, 0.6f);
            yield return new ContextActionCommand(ctx =>
            {
                ctx.DialogCtl.StartClipDialogue(Scene1DialogueId.RunAwayIntro, () =>
                {
                    ctx.CutsceneCtl.RunAwayVoiceOver();
                });
            });

        }
    }
}
