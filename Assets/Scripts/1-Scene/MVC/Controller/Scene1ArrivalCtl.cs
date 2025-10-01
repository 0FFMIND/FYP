using Manager;
using UnityEngine;

namespace MVC
{
    public class Scene1ArrivalCtl : MonoBehaviour
    {
        [SerializeField]
        private TimelineDialogCtl dialogCtl;

        private PlayerCtl player;

        void Start()
        {
            // ½ûÖ¹playerÒÆ¶¯
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtl>();
            player.model.SetDisabled(true);
            // Òþ²Ødialog
            dialogCtl.HideDialogue();
        }
    }
}
