
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MVC
{
    public class Scene1CutsceneContext
    {
        public PlayerScriptMoveCtl Mover;
        public PlayerEmoteCtl Emote;
        public PlayableDirector Director;
        public TimelineDialogCtl Dialog;
        public TimelineDialogCtl DialogSide;
        public TimelineDialogCtl UI;
        public CameraSwitch Switcher;
        public GuideDialogCtl Guide;
        public ParallaxBG BG;
        public CameraCtl Camera;
        public GameObject Door;
        public GameObject Flower;
        public List<Sprite> CloseDoorSprites;
    }
}
