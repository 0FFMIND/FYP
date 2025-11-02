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
        private List<Sprite> closeDoor;

        [SerializeField]
        private GuideDialogCtl guideCtl;

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
            EventBus.Publish(new EScene1ArrivalStateChange(Scene1State.GoToBoard));
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

        public void MoveBack(float y, float time)
        {
            Vector3 pos = mover.gameObject.transform.position;
            pos.y -= y;
            mover.StartMove(pos, time, Direction.Up, null);
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
                            new EScene1ArrivalStateChange(Scene1State.AwaitMenuToggle)
                        );
                        PauseMgr.Instance.SetShowGuide(true);
                    }
                );
            }
        }

        private void ResumeDirector()
        {
            director.Resume();
        }
    }
}
