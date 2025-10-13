using Manager;
using UnityEngine;

namespace MVC
{
    public class Scene1Stone : MonoBehaviour
    {
        [SerializeField]
        InventoryDialogCtl dialogCtl;

        [SerializeField]
        string[] lines;

        public void AddCoin()
        {
            // ½ûÖ¹playerÒÆ¶¯
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtl>();
            player.model.SetDisabled(true);
            InventoryMgr.Instance.AddById("coin", 1);
            dialogCtl.StartDialogue(lines, CanMove);
        }

        private void CanMove() {
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtl>();
            player.model.SetDisabled(false);
        }
    }
}
