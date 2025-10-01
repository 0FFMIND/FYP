using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace MVC
{
    public class PlayerMoveSignal : MonoBehaviour
    {
        private PlayerScriptMoveCtl mover;

        [SerializeField]
        private PlayableDirector director;

        [SerializeField]
        private TimelineDialogCtl dialogCtl;

        private void Start()
        {
            mover = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScriptMoveCtl>();
        }

        public void MoveBack()
        {
            Vector3 pos = mover.gameObject.transform.position;
            pos.y -= 0.6f;
            mover.StartMove(pos, 1.5f, Direction.Up, null);
        }

        public void FirstDialog()
        {
            // ÔÝÍ£director
            director.Pause();
            // Æô¶¯dialog
            dialogCtl.StartFirstDialogue(ResumeDirector);
        }
        public void PlayerSecondMove()
        {
            StartCoroutine(SecondMoveThenChat());
        }

        public IEnumerator SecondMoveThenChat()
        {
            PlayerSecondTurn();
            yield return new WaitForSecondsRealtime(0.1f);
            // ÔÝÍ£director
            director.Pause();
            dialogCtl.StartSecondDialogue(a);
        }

        private void PlayerSecondTurn()
        {
            Vector3 pos = mover.gameObject.transform.position;
            pos.y -= 0.7f;
            mover.StartMove(pos, 2f, Direction.Down, null);
        }

        private void a()
        {
            mover.gameObject.GetComponent<PlayerCtl>().model.SetDisabled(false);
        }

        private void ResumeDirector()
        {
            director.Resume();
        }
    }
}
