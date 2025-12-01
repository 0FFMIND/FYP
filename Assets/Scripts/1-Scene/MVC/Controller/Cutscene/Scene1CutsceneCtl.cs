using UnityEngine;
namespace MVC
{
    public class Scene1CutsceneCtl : MonoBehaviour
    {
        [SerializeField]
        private Scene1CutsceneContext ctx;

        private Scene1CutsceneRunner runner;

        private void Start()
        {
            runner = new Scene1CutsceneRunner();
        }

        public void CloseDoor()
        {
            var closeDoorCmd = new Scene1CloseDoor().Build();
            StartCoroutine(runner.Run(ctx, closeDoorCmd));
        }

        public void MoveBack()
        {
            var moveBackCmd = new OffsetMoveCommand(0f, -0.6f, 1.5f, Direction.Up);
            StartCoroutine(runner.Run(ctx, moveBackCmd));
        }

        public void TimelineIntroDialog()
        {
            // 暂停director
            ctx.Director.Pause();
            // 启动dialog
            ctx.DialogCtl.StartClipDialogue(Scene1DialogueId.TimelineIntro, () =>
            {
                // 恢复director
                ctx.Director.Resume();
            });
        }

        public void LookAroundThenTalk()
        {
            var lookAroundCmd = new Scene1LookAroundThenTalk().Build();
            StartCoroutine(runner.Run(ctx, lookAroundCmd));
        }

        public void MoveBackFromSignThenTalk()
        {
            var moveBackFromSignCmd = new Scene1MoveBackFromSignThenTalk().Build();
            StartCoroutine(runner.Run(ctx, moveBackFromSignCmd));
        }

        public void RooftopExploreCompleted()
        {
            var rooftopExploreCompletedCmd = new Scene1RooftopExploreCompleted().Build();
            StartCoroutine(runner.Run(ctx, rooftopExploreCompletedCmd));
        }

        public void MeadowExplore()
        {
            var meadowExploreCmd = new Scene1MeadowExplore().Build();
            StartCoroutine(runner.Run(ctx, meadowExploreCmd));
        }

        public void SearchMeadow()
        {
            var searchMeadowCmd = new Scene1SearchMeadow().Build();
            StartCoroutine(runner.Run(ctx, searchMeadowCmd));
        }

        public void RunAwayIntro()
        {
            var runAwayIntroCmd = new Scene1RunAwayIntro().Build();
            StartCoroutine(runner.Run(ctx, runAwayIntroCmd));
        }

        public void RunAwayVoiceOver()
        {
            var runAwayVoiceOverCmd = new Scene1RunAwayVoiceOver().Build();
            StartCoroutine(runner.Run(ctx, runAwayVoiceOverCmd));
        }

        public void KeyTurning()
        {
            var keyTurningCmd = new Scene1KeyTurning().Build();
            StartCoroutine(runner.Run(ctx, keyTurningCmd));
        }

        public void RunToDoor()
        {
            var runToDoorCmd = new Scene1RunToDoor().Build();
            StartCoroutine(runner.Run(ctx, runToDoorCmd));
        }

    }
}
