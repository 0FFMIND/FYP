using System.Collections;
using System.Collections.Generic;
using Manager;
using Utils;

namespace MVC
{
    /// <summary>
    /// 玩家逃跑环境对白这一小段过场的命令序列
    /// </summary>
    public class Scene1RunAwayVoiceOver
    {

        public IEnumerable<IScene1CutsceneCommand> Build()
        {
            EventBus.Publish(new EJournalStepChanged("endRooftop", 2, StepState.Done));
            yield return new CameraPanToYCommand(-4f, 3f);
            yield return new WaitCommand(2f);
            yield return new ContextActionCommand(ctx =>
            {
                ctx.DialogSideCtl.StartClipDialogue(Scene1DialogueId.RunAwayVoiceOver, () =>
                {
                    ctx.CutsceneCtl.KeyTurning();
                });
            });
        }
    }
}
