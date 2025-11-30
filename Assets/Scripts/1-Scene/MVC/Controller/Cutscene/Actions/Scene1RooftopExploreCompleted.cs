using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace MVC
{
    /// <summary>
    /// 玩家在天台探索完成后的过场
    /// </summary>
    public class Scene1RooftopExploreCompleted
    {

        public IEnumerable<IScene1CutsceneCommand> Build()
        {
            yield return new WaitCommand(0.5f);
            yield return new JumpCommand(0.2f, 0.6f);
            yield return new WaitCommand(0.5f);
            yield return new ContextActionCommand(ctx =>
            {
                ctx.BG.isOn = false;
                ctx.DialogCtl.StartClipDialogue(Scene1DialogueId.RooftopExploreEnd, () =>
                {
                    GameObject
                        .FindGameObjectWithTag("Player")
                        .GetComponent<PlayerCtl>()
                        .model.SetDisabled(false);
                    EventBus.Publish(new EJournalStatusChanged("endRooftop", JournalStatus.Active));
                    EventBus.Publish(new EScene1ArrivalPhaseChange(Scene1Phase.MeadowExplore));
                    ctx.BG.isOn = true;
                });
            });

        }
    }
}
