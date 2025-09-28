using UnityEngine;
using UnityEngine.Playables;

namespace MVC
{
    public class PlayerMoveSignal : MonoBehaviour
    {
        private PlayerScriptMoveCtl mover;
        [SerializeField] private PlayableDirector director;
        [SerializeField] private TimelineDialogCtl dialogCtl;

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
            director.Stop();
            // Æô¶¯dialog
            dialogCtl.StartFirstDialogue();
        }
    }
}
