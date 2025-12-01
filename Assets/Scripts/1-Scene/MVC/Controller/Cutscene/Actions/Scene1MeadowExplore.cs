using System.Collections.Generic;

namespace MVC
{
    /// <summary>
    /// 玩家刚到达草甸的过场
    /// </summary>
    public class Scene1MeadowExplore
    {

        public IEnumerable<IScene1CutsceneCommand> Build()
        {

            yield return new ContextActionCommand(ctx =>
            {
                ctx.EmoteCtl.Stop();
            });
            yield return new JumpCommand(0.2f, 0.6f);
            yield return new EmotePlayCommand(EmoteType.Warning, 0.6f, true);
            yield return new WaitCommand(1f);
            yield return new ContextActionCommand(ctx =>
            {
                ctx.BG.isOn = false;
                ctx.DialogCtl.StartClipDialogue(Scene1DialogueId.MeadowExploreIntro, () =>
                {
                    ctx.BG.isOn = true;
                    // 开始播放动画
                    ctx.CutsceneCtl.SearchMeadow();
                });
            });
        }
    }
}
