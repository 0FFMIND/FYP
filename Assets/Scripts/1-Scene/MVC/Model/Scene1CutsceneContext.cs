
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MVC
{
    public class Scene1CutsceneContext : MonoBehaviour
    {
        public PlayerScriptMoveCtl Mover;
        public PlayerEmoteCtl EmoteCtl;
        public PlayableDirector Director;
        public TimelineDialogCtl DialogCtl;
        public TimelineDialogCtl DialogSideCtl;
        public TimelineDialogCtl UICtl;
        public CameraSwitch Switcher;
        public GuideDialogCtl GuideCtl;
        public ParallaxBG BG;
        public CameraCtl Camera;
        public GameObject Door;
        public GameObject Flower;
        public List<Sprite> CloseDoorSprites;
    }
}
