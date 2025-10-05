using System.Collections;
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

        private void Start()
        {
            mover = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScriptMoveCtl>();
            emoteCtl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEmoteCtl>();
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
            yield return new WaitForSecondsRealtime(0.2f);
            PlayerSecondTurn();
            yield return new WaitForSecondsRealtime(1f);
            mover.SetFace(Direction.Right);
            yield return new WaitForSecondsRealtime(1f);
            mover.SetFace(Direction.Left);
            yield return new WaitForSecondsRealtime(1f);
            mover.SetFace(Direction.Down);
            yield return new WaitForSecondsRealtime(1f);
            emoteCtl.Play(EmoteType.Thinking, 1f);
            yield return new WaitForSecondsRealtime(1.5f);
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

        private void ResumeDirector()
        {
            director.Resume();
        }
    }
}
