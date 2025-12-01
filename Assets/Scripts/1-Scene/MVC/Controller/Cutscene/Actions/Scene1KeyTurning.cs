using System.Collections.Generic;
using Manager;

namespace MVC
{
    /// <summary>
    /// 钥匙转动这一小段过场的命令序列
    /// </summary>
    public class Scene1KeyTurning
    {

        public IEnumerable<IScene1CutsceneCommand> Build()
        {
            AudioMgr.Instance.PlaySFX("keyTurning");
            yield return new CameraPanToYCommand(4f, 1f);
            yield return new WaitCommand(1f);
            yield return new ContextActionCommand(ctx =>
            {
                ctx.BG.isOn = false;
                ctx.DialogCtl.StartClipDialogue(Scene1DialogueId.RunAwayMid, () =>
                {
                    ctx.CameraCtl.FollowPlayer();
                    ctx.BG.isOn = true;
                    ctx.CutsceneCtl.RunToDoor();
                });
            });

        }
    }
}
