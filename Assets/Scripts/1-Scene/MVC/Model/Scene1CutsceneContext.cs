using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MVC
{
    public class Scene1CutsceneContext : MonoBehaviour
    {
        public Scene1CutsceneCtl CutsceneCtl;
        public PlayerScriptMoveCtl Mover;
        public PlayerEmoteCtl EmoteCtl;
        public PlayableDirector Director;
        public TimelineDialogCtl DialogCtl;
        public TimelineDialogCtl DialogSideCtl;
        public TimelineDialogCtl UICtl;
        public CameraSwitch Switcher;
        public GuideDialogCtl GuideCtl;
        public ParallaxBG BG;
        public CameraCtl CameraCtl;
        public GameObject Door;
        public GameObject Flower;
        public GameObject DoorGO;
        public GameObject OpenDoor;
        public List<Sprite> CloseDoorSprites;
        public float StepX = 1f; // 每次移动的步长（世界坐标X）
        public float StepTime = 0.25f; // 每步移动所用时长（秒）
        public float LeftX = -11.9f; // 最左X
        public float RightX = -6.5f; // 最右X
    }
}
