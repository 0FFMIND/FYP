using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.Playables;
using Utils;

namespace MVC
{
    public class PlayerMoveSignal : MonoBehaviour
    {
        private PlayerScriptMoveCtl mover;

        private PlayerEmoteCtl emoteCtl;

        [SerializeField]
        private PlayableDirector director;

        [SerializeField]
        private TimelineDialogCtl dialogCtl;

        [SerializeField]
        private TimelineDialogCtl dialogSideCtl;

        [SerializeField]
        private TimelineDialogCtl UICtl;

        [SerializeField]
        CameraSwitch switcher;

        [SerializeField]
        private List<Sprite> closeDoor;

        [SerializeField]
        private GuideDialogCtl guideCtl;

        [SerializeField]
        private ParallaxBG bg;

        [SerializeField]
        private CameraCtl cameraCtl;

        [SerializeField]
        private GameObject door;

        [SerializeField]
        private GameObject flower;

        [SerializeField]
        private float stepX = 0.40f; // 每次移动的步长（世界坐标X）

        [SerializeField]
        private float stepTime = 0.25f; // 每步移动所用时长（秒）

        [SerializeField]
        private float leftX = -12f; // 最左X

        [SerializeField]
        private float rightX = -7f; // 最右X

        [SerializeField]
        private float finalX = -10.69f; // 最终停在这里

        private void Start()
        {
            mover = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScriptMoveCtl>();
            emoteCtl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEmoteCtl>();
        }

        public void CloseDoor()
        {
            StartCoroutine(PlayerCloseDoor());
        }

        private IEnumerator PlayerCloseDoor()
        {
            mover.SetLock(true);
            AudioManager.Instance.PlaySFX("dooropen");
            mover.SetSprite(closeDoor[0]);
            yield return new WaitForSecondsRealtime(0.15f);
            mover.SetSprite(closeDoor[1]);
            yield return new WaitForSecondsRealtime(0.15f);
            mover.SetSprite(closeDoor[0]);
            yield return new WaitForSecondsRealtime(0.15f);
            mover.SetSprite(closeDoor[2]);
            yield return new WaitForSecondsRealtime(0.15f);
            mover.SetLock(false);
        }

        public void MoveBack()
        {
            Vector3 pos = mover.gameObject.transform.position;
            pos.y -= 0.6f;
            mover.StartMove(pos, 1.5f, Direction.Up, null);
        }

        public void FirstDialog()
        {
            // 暂停director
            director.Pause();
            // 启动dialog
            dialogCtl.StartFirstDialogue(ResumeDirector);
        }

        public void PlayerSecondMove()
        {
            StartCoroutine(SecondMoveThenChat());
        }

        private IEnumerator SecondMoveThenChat()
        {
            mover.SetFace(Direction.Down);
            yield return new WaitForSecondsRealtime(0.4f);
            PlayerSecondTurn();
            yield return new WaitForSecondsRealtime(1f);
            mover.SetFace(Direction.Right);
            yield return new WaitForSecondsRealtime(1f);
            mover.SetFace(Direction.Left);
            yield return new WaitForSecondsRealtime(1f);
            mover.SetFace(Direction.Down);
            yield return new WaitForSecondsRealtime(1f);
            emoteCtl.Play(EmoteType.Thinking, 1f);
            yield return new WaitForSecondsRealtime(1.7f);
            // 暂停director
            director.Pause();
            dialogCtl.StartSecondDialogue(PlayerSecondMoveEnd);
        }

        private void PlayerSecondTurn()
        {
            Vector3 pos = mover.gameObject.transform.position;
            pos.y -= 0.7f;
            mover.StartMove(pos, 1.6f, Direction.Down, null);
        }

        private void PlayerSecondMoveEnd()
        {
            // 停止Timeline过场动画
            director.Stop();
            // 改变当前scene1状态机状态
            EventBus.Publish(new EScene1ArrivalPhaseChange(Scene1Phase.SignTutorial));
        }

        public IEnumerator PlayerThirdMove()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            if (mover.anim.currentDir != Direction.Up)
            {
                mover.SetFace(Direction.Up);
                yield return new WaitForSecondsRealtime(0.5f);
            }
            else
            {
                yield return new WaitForSecondsRealtime(0.5f);
            }
            yield return mover.Jump(0.2f, 0.6f);
            yield return new WaitForSecondsRealtime(0.5f);
            MoveBack(1.2f, 2f);
            yield return new WaitForSecondsRealtime(1f);
            emoteCtl.Play(EmoteType.Thinking, 1f);
            yield return new WaitForSecondsRealtime(1.7f);
            dialogCtl.StartThirdDialogue(PlayerThirdMoveEnd);
        }

        private void PlayerThirdMoveEnd()
        {
            if (guideCtl != null)
            {
                guideCtl.StartDialogue(
                    "1-Scene-6.txt",
                    () =>
                    {
                        // 改变当前scene1状态机状态
                        EventBus.Publish(
                            new EScene1ArrivalPhaseChange(Scene1Phase.AwaitMenuToggle)
                        );
                        PauseMgr.Instance.SetShowGuide(true);
                    }
                );
            }
        }

        public IEnumerator PlayerFourthMove()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            yield return mover.Jump(0.2f, 0.6f);
            yield return new WaitForSecondsRealtime(0.5f);
            bg.isOn = false;
            dialogCtl.StartSixthDialogue(() =>
            {
                GameObject
                    .FindGameObjectWithTag("Player")
                    .GetComponent<PlayerCtl>()
                    .model.SetDisabled(false);
                EventBus.Publish(new EJournalStatusChanged("endRooftop", JournalStatus.Active));
                EventBus.Publish(new EScene1ArrivalPhaseChange(Scene1Phase.MeadowExplore));
                bg.isOn = true;
            });
        }

        public IEnumerator PlayerFifthMove()
        {
            emoteCtl.Stop();
            yield return mover.Jump(0.2f, 0.6f);
            emoteCtl.Play(EmoteType.Warning, 0.6f, true);
            yield return new WaitForSecondsRealtime(1f);
            bg.isOn = false;
            dialogCtl.StartSeventhDialogue(() =>
            {
                bg.isOn = true;
                // 开始播放动画
                StartCoroutine(PlayerSixthMove());
            });
        }

        public IEnumerator PlayerSixthMove()
        {
            var t = mover.transform;
            float x = t.position.x;

            // 情况1：当前在右边界之外 ⇒ 不去 rightX，直接去 leftX → finalX
            if (x < rightX - stepX)
            {
                yield return StepMoveToX(rightX, false);
            }

            // 2) 再向左到最左
            yield return StepMoveToX(leftX, true);

            var target = new Vector3(
                mover.transform.position.x + stepX,
                mover.transform.position.y,
                mover.transform.position.z
            );
            mover.StartMove(target, 2.3f, Direction.Right, null);
            yield return new WaitForSecondsRealtime(0.5f);
            mover.SetFace(Direction.Up);
            yield return new WaitForSecondsRealtime(0.1f);
            yield return mover.Jump(0.2f, 0.6f);
            emoteCtl.Play(EmoteType.Warning, 0.6f, true);
            yield return new WaitForSecondsRealtime(1f);
            dialogCtl.StartEighthDialogue(() =>
            {
                EventBus.Publish(new EJournalStepChanged("endRooftop", 1, StepState.Done));
                switcher.EnterUI(() =>
                {
                    AudioManager.Instance.PlayBGM("1-bgm-2", 0f);
                    UICtl.StartNinethDialogue(() =>
                    {
                        switcher.ExitUI(() =>
                        {
                            StartCoroutine(PlayerSeventhMove());
                        });
                    });
                });
            });
        }

        public IEnumerator PlayerSeventhMove()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            emoteCtl.Play(EmoteType.Warning, 0.6f, true);
            yield return mover.Jump(0.2f, 0.6f);
            dialogCtl.StartTenthDialogue(() =>
            {
                StartCoroutine(PlayerEighthMove());
            });
        }

        public IEnumerator PlayerEighthMove()
        {
            EventBus.Publish(new EJournalStepChanged("endRooftop", 2, StepState.Done));
            cameraCtl.PanToY(-3f, 3f);
            yield return new WaitForSecondsRealtime(2f);
            dialogSideCtl.StartEleventhDialogue(() =>
            {
                StartCoroutine(PlayerNinethMove());
            });
        }

        public IEnumerator PlayerNinethMove()
        {
            AudioManager.Instance.PlaySFX("keyTurning");
            cameraCtl.PanToY(3f, 1f);
            yield return new WaitForSecondsRealtime(1f);
            bg.isOn = false;
            dialogCtl.StartTwelfthDialogue(() =>
            {
                cameraCtl.FollowPlayer();
                StartCoroutine(PlayerTenthMove());
                bg.isOn = true;
            });
        }

        public IEnumerator PlayerTenthMove()
        {
            flower.SetActive(false);
            door.SetActive(false);
            mover.SetFace(Direction.Down);
            cameraCtl.ZoomOrthoBy(0.7f, 0.2f);
            yield return new WaitForSecondsRealtime(0.5f);
            var target = new Vector3(mover.transform.position.x, 0.4f, mover.transform.position.z);
            mover.StartMove(target, 6f, Direction.Down, null);
            yield return new WaitForSecondsRealtime(0.85f);
            mover.SetFace(Direction.Right);
            yield return new WaitForSecondsRealtime(0.15f);
            target = new Vector3(6.88f, mover.transform.position.y, mover.transform.position.z);
            mover.StartMove(target, 6f, Direction.Right, null);
            yield return new WaitForSecondsRealtime(3.5f);
            mover.SetFace(Direction.Up);
            yield return new WaitForSecondsRealtime(0.2f);
            target = new Vector3(mover.transform.position.x, 2.13f, mover.transform.position.z);
            mover.StartMove(target, 6f, Direction.Up, null);
            yield return new WaitForSecondsRealtime(1f);
            AudioManager.Instance.PlaySFX("dooropen");
            switcher.EnterUI(() =>
            {
                AudioManager.Instance.PlayBGM("1-bgm-4", 0f);
                UICtl.StartThirteenthDialogue(() =>
                {
                    AudioManager.Instance.StopBGM();
                    // 进入1-Scene-UI
                    EventBus.Publish(
                        new ESceneFade(
                            toScene: "Title-Scene",
                            fadeOutDuration: 0.5f,
                            fadeInDuration: 1f
                        )
                    );
                });
            });
        }

        // 按固定步长把玩家在水平线上推进到 targetX；自动设置朝向并等待每步动画完成
        private IEnumerator StepMoveToX(float targetX, bool isLeft)
        {
            var current = mover.transform.position.x;
            while (Mathf.Abs(targetX - current) > stepX)
            {
                var target = new Vector3(
                    isLeft
                        ? mover.transform.position.x - stepX
                        : mover.transform.position.x + stepX,
                    mover.transform.position.y,
                    mover.transform.position.z
                );
                mover.StartMove(target, 2.3f, isLeft ? Direction.Left : Direction.Right, null);
                yield return new WaitForSecondsRealtime(0.5f);
                mover.SetFace(Direction.Up);
                yield return new WaitForSecondsRealtime(0.05f);
                AudioManager.Instance.PlaySFX("warning");
                emoteCtl.Play(EmoteType.Error, 0.6f, true);
                yield return new WaitForSecondsRealtime(1f);

                current = mover.transform.position.x;
            }
        }

        public void MoveBack(float y, float time)
        {
            Vector3 pos = mover.gameObject.transform.position;
            pos.y -= y;
            mover.StartMove(pos, time, Direction.Up, null);
        }

        private void ResumeDirector()
        {
            director.Resume();
        }
    }
}
