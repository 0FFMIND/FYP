using UnityEngine;
using Utils;

namespace MVC
{
    public class Scene1TimelinePhase : IScene1PhaseHandler
    {
        private readonly Scene1PhaseCtl ctl;

        public Scene1TimelinePhase(Scene1PhaseCtl ctl) => this.ctl = ctl;

        public void Enter()
        {
            // 设置人物位置
            var player = GameObject.FindGameObjectWithTag("Player");
            EventBus.Publish(new EJournalStepChanged("reachRooftop", 0, StepState.Done));
            player.transform.position = ctl.InitPos;
            // 启动timeline
            if (ctl.Director != null)
            {
                ctl.Director.time = 0;
                ctl.Director.Play();
            }
        }

        public void OnJournalChanged(EJournalStatusChanged e) { }

        public void OnPauseChanged(EPauseChanged e) { }

        public void Tick() { }
    }
}
