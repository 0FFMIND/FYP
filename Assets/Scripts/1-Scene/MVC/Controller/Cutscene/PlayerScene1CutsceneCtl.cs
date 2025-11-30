using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.Playables;
using Utils;

namespace MVC
{
    public class PlayerScene1CutsceneCtl : MonoBehaviour
    {
        [SerializeField]
        private Scene1CutsceneContext ctx;

        private Scene1CutsceneRunner runner;

        [SerializeField]
        private float stepX = 0.40f; // 每次移动的步长（世界坐标X）

        [SerializeField]
        private float stepTime = 0.25f; // 每步移动所用时长（秒）

        [SerializeField]
        private float leftX = -12f; // 最左X

        [SerializeField]
        private float rightX = -7f; // 最右X

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

        // public IEnumerator PlayerFifthMove()
        // {
        //     emoteCtl.Stop();
        //     yield return mover.Jump(0.2f, 0.6f);
        //     emoteCtl.Play(EmoteType.Warning, 0.6f, true);
        //     yield return new WaitForSecondsRealtime(1f);
        //     bg.isOn = false;
        //     dialogCtl.StartClipDialogue(Scene1DialogueId.MeadowExploreIntro, () =>
        //     {
        //         bg.isOn = true;
        //         // 开始播放动画
        //         StartCoroutine(PlayerSixthMove());
        //     });
        // }

        // public IEnumerator PlayerSixthMove()
        // {
        //     var t = mover.transform;
        //     float x = t.position.x;

        //     // 情况1：当前在右边界之外 ⇒ 不去 rightX，直接去 leftX → finalX
        //     if (x < rightX - stepX)
        //     {
        //         yield return StepMoveToX(rightX, false);
        //     }

        //     // 2) 再向左到最左
        //     yield return StepMoveToX(leftX, true);

        //     var target = new Vector3(
        //         mover.transform.position.x + stepX,
        //         mover.transform.position.y,
        //         mover.transform.position.z
        //     );
        //     mover.StartMove(target, 2.3f, Direction.Right, null);
        //     yield return new WaitForSecondsRealtime(0.5f);
        //     mover.SetFace(Direction.Up);
        //     yield return new WaitForSecondsRealtime(0.1f);
        //     yield return mover.Jump(0.2f, 0.6f);
        //     emoteCtl.Play(EmoteType.Warning, 0.6f, true);
        //     yield return new WaitForSecondsRealtime(1f);
        //     dialogCtl.StartClipDialogue(Scene1DialogueId.MeadowExploreMid, () =>
        //     {
        //         EventBus.Publish(new EJournalStepChanged("endRooftop", 1, StepState.Done));
        //         switcher.EnterUI(() =>
        //         {
        //             AudioMgr.Instance.PlayBGM("1-bgm-2", 0f);
        //             UICtl.StartClipDialogue(Scene1DialogueId.MeadowExploreEnd, () =>
        //             {
        //                 switcher.ExitUI(() =>
        //                 {
        //                     StartCoroutine(PlayerSeventhMove());
        //                 });
        //             });
        //         });
        //     });
        // }

        // public IEnumerator PlayerSeventhMove()
        // {
        //     yield return new WaitForSecondsRealtime(0.1f);
        //     emoteCtl.Play(EmoteType.Warning, 0.6f, true);
        //     yield return mover.Jump(0.2f, 0.6f);
        //     dialogCtl.StartClipDialogue(Scene1DialogueId.RunAwayIntro, () =>
        //     {
        //         StartCoroutine(PlayerEighthMove());
        //     });
        // }

        // public IEnumerator PlayerEighthMove()
        // {
        //     EventBus.Publish(new EJournalStepChanged("endRooftop", 2, StepState.Done));
        //     cameraCtl.PanToY(-4f, 3f);
        //     yield return new WaitForSecondsRealtime(2f);
        //     dialogSideCtl.StartClipDialogue(Scene1DialogueId.RunAwayVoiceOver, () =>
        //     {
        //         StartCoroutine(PlayerNinethMove());
        //     });
        // }

        // public IEnumerator PlayerNinethMove()
        // {
        //     AudioMgr.Instance.PlaySFX("keyTurning");
        //     cameraCtl.PanToY(4f, 1f);
        //     yield return new WaitForSecondsRealtime(1f);
        //     bg.isOn = false;
        //     dialogCtl.StartClipDialogue(Scene1DialogueId.RunAwayMid, () =>
        //     {
        //         cameraCtl.FollowPlayer();
        //         StartCoroutine(PlayerTenthMove());
        //         bg.isOn = true;
        //     });
        // }

        // public IEnumerator PlayerTenthMove()
        // {
        //     flower.SetActive(false);
        //     door.SetActive(false);
        //     mover.SetFace(Direction.Down);
        //     cameraCtl.ZoomOrthoBy(0.7f, 0.2f);
        //     yield return new WaitForSecondsRealtime(0.5f);
        //     var target = new Vector3(mover.transform.position.x, 0.4f, mover.transform.position.z);
        //     mover.StartMove(target, 6f, Direction.Down, null);
        //     yield return new WaitForSecondsRealtime(0.85f);
        //     mover.SetFace(Direction.Right);
        //     yield return new WaitForSecondsRealtime(0.15f);
        //     target = new Vector3(6.88f, mover.transform.position.y, mover.transform.position.z);
        //     mover.StartMove(target, 6f, Direction.Right, null);
        //     yield return new WaitForSecondsRealtime(3.5f);
        //     mover.SetFace(Direction.Up);
        //     yield return new WaitForSecondsRealtime(0.2f);
        //     target = new Vector3(mover.transform.position.x, 2.13f, mover.transform.position.z);
        //     mover.StartMove(target, 6f, Direction.Up, null);
        //     yield return new WaitForSecondsRealtime(1f);
        //     AudioMgr.Instance.PlaySFX("dooropen");
        //     switcher.EnterUI(() =>
        //     {
        //         AudioMgr.Instance.PlayBGM("1-bgm-4", 0f);
        //         UICtl.StartClipDialogue(Scene1DialogueId.RunAwayEnd, () =>
        //         {
        //             AudioMgr.Instance.StopBGM();
        //             // 进入1-Scene-UI
        //             EventBus.Publish(
        //                 new ESceneFade(
        //                     toScene: "Title-Scene",
        //                     fadeOutDuration: 0.5f,
        //                     fadeInDuration: 1f
        //                 )
        //             );
        //         });
        //     });
        // }

        // // 按固定步长把玩家在水平线上推进到 targetX；自动设置朝向并等待每步动画完成
        // private IEnumerator StepMoveToX(float targetX, bool isLeft)
        // {
        //     var current = mover.transform.position.x;
        //     while (Mathf.Abs(targetX - current) > stepX)
        //     {
        //         var target = new Vector3(
        //             isLeft
        //                 ? mover.transform.position.x - stepX
        //                 : mover.transform.position.x + stepX,
        //             mover.transform.position.y,
        //             mover.transform.position.z
        //         );
        //         mover.StartMove(target, 2.3f, isLeft ? Direction.Left : Direction.Right, null);
        //         yield return new WaitForSecondsRealtime(0.5f);
        //         mover.SetFace(Direction.Up);
        //         yield return new WaitForSecondsRealtime(0.05f);
        //         AudioMgr.Instance.PlaySFX("warning");
        //         emoteCtl.Play(EmoteType.Error, 0.6f, true);
        //         yield return new WaitForSecondsRealtime(1f);

        //         current = mover.transform.position.x;
        //     }
        // }

    }
}
